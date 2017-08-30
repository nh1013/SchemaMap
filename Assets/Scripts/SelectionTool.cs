using UnityEngine;
using System.Collections;

public class SelectionTool : MonoBehaviour
{
    public Transform PreviewBeamPrefab;

    public ControlPanel controlPanel;
    private SteamVR_TrackedController m_controller;

    private Transform m_previewBeam;
    private Vector3 m_startNode;
    private float m_startDistance;

    private Transform m_startObject;
    private Transform m_finishObject;

    // Use this for initialization
    void Start() {
        m_controller = GetComponent<SteamVR_TrackedController>();

        m_controller.TriggerClicked += new ClickedEventHandler(TriggerDown);
        m_controller.TriggerUnclicked += new ClickedEventHandler(TriggerUp);
    }

    private void TriggerDown(object sender, ClickedEventArgs e) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            string hitTag = hit.transform.tag;
            m_startObject = hit.transform;
            if (hitTag == "SourceFieldCell") {
                m_startNode = new Vector3(
                    m_startObject.position.x + 0.5f * m_startObject.lossyScale.x,
                    m_startObject.position.y,
                    m_startObject.position.z);
            }
            else if (hitTag == "TargetFieldCell") {
                m_startNode = new Vector3(
                    m_startObject.position.x - 0.5f * m_startObject.lossyScale.x,
                    m_startObject.position.y,
                    m_startObject.position.z);
            }
            else if (hitTag == "MappingBeam") {
                return;
            }
            else {
                // ignore invalid targets
                m_startObject = null;
                return;
            }

            // create preview beam
            m_previewBeam = Instantiate(PreviewBeamPrefab, transform);
            m_startDistance = hit.distance;
            m_previewBeam.position = Vector3.Lerp(m_startNode, hit.point, 0.5f);
            m_previewBeam.LookAt(hit.point);
            m_previewBeam.localScale = new Vector3(
                m_previewBeam.localScale.x,
                m_previewBeam.localScale.y,
                Vector3.Distance(m_startNode, hit.point)
            );
            return;
        }
    }

    private void TriggerUp(object sender, ClickedEventArgs e) {
        if (!m_startObject) {
            m_finishObject = null;
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            m_finishObject = hit.transform;
            if (m_startObject == m_finishObject || m_startObject.tag == "MappingBeam") {
                controlPanel.Select(m_startObject);
            }
            else if ( (m_startObject.tag == "SourceFieldCell" && m_finishObject.tag == "TargetFieldCell") ||
                      (m_startObject.tag == "TargetFieldCell" && m_finishObject.tag == "SourceFieldCell") ) {
                controlPanel.Select(m_startObject);
                controlPanel.Select(m_finishObject);
            }
        }
        m_startObject = null;
        m_finishObject = null;
        if (m_previewBeam) {
            Destroy(m_previewBeam);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!m_controller.triggerPressed) {
            return;
        }
        if (!m_previewBeam) {
            return;
        }

        Vector3 endPos;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            Transform targetObject = hit.transform;
            if (m_startObject.tag == "SourceFieldCell" && targetObject.tag == "TargetFieldCell") {
                endPos = new Vector3(
                    targetObject.position.x + 0.5f * targetObject.lossyScale.x,
                    targetObject.position.y,
                    targetObject.position.z
                );
            }
            else if (m_startObject.tag == "TargetFieldCell" && targetObject.tag == "SourceFieldCell") {
                endPos = new Vector3(
                    targetObject.position.x - 0.5f * targetObject.lossyScale.x,
                    targetObject.position.y,
                    targetObject.position.z
                );
            }
            else {
                endPos = hit.point;
            }
        }
        else {
            endPos = transform.position + m_startDistance * Vector3.forward;
        }

        // adjust preview beam
        m_previewBeam.position = Vector3.Lerp(m_startNode, endPos, 0.5f);
        m_previewBeam.LookAt(endPos);
        m_previewBeam.localScale = new Vector3(
            m_previewBeam.localScale.x,
            m_previewBeam.localScale.y,
            Vector3.Distance(m_startNode, endPos)
        );
        return;
    }
}
