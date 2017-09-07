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

    private Transform m_startObject = null;
    private Transform m_finishObject = null;

    private Transform m_lastObject = null;

    // Use this for initialization
    void Start() {
        m_controller = GetComponent<SteamVR_TrackedController>();

        m_controller.TriggerClicked += new ClickedEventHandler(TriggerDown);
        m_controller.TriggerUnclicked += new ClickedEventHandler(TriggerUp);
    }

    private void TriggerDown(object sender, ClickedEventArgs e) {
        //Debug.Log("Trigger down detected");
        m_startObject = null;
        if (m_previewBeam) {
            Destroy(m_previewBeam.gameObject);
            m_previewBeam = null;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            //Debug.Log("Raycast hit item:" + hit.transform.name);
            // parent of collider holds script and tag, collider is model of item
            m_startObject = hit.transform.parent;
            if (m_startObject == null) {
                // ignore invalid targets
                return;
            }
            switch (m_startObject.tag) {
                case "SourceFieldCell":
                    m_startNode = hit.transform.position + 0.5f * hit.transform.right * hit.transform.lossyScale.x;
                    break;
                case "TargetFieldCell":
                    m_startNode = hit.transform.position - 0.5f * hit.transform.right * hit.transform.lossyScale.x;
                    break;
                case "MappingBeam":
                    return;
                default:
                    return;
            }

            // create preview beam
            m_previewBeam = Instantiate(PreviewBeamPrefab);
            m_startDistance = hit.distance;
            m_previewBeam.position = Vector3.Lerp(m_startNode, hit.point, 0.5f);
            m_previewBeam.LookAt(hit.point);
            m_previewBeam.localScale = new Vector3(
                m_previewBeam.localScale.x,
                m_previewBeam.localScale.y,
                Vector3.Distance(m_startNode, hit.point)
            );
        }
    }

    private void TriggerUp(object sender, ClickedEventArgs e) {
        //Debug.Log("Trigger up detected");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            //Debug.Log("Raycast hit item:" + hit.transform.name);
            // parent of collider holds script and tag, collider is model of item
            m_finishObject = hit.transform.parent;

            if (!m_startObject || !m_finishObject) {
                controlPanel.Select(null);
            }
            else if (m_startObject == m_finishObject) {
                controlPanel.Select(m_startObject);
            }
            else if ( (m_startObject.tag == "SourceFieldCell" && m_finishObject.tag == "TargetFieldCell") ||
                      (m_startObject.tag == "TargetFieldCell" && m_finishObject.tag == "SourceFieldCell") ) {
                controlPanel.Select(m_startObject);
                controlPanel.Select(m_finishObject);
                controlPanel.LinkFields();
            }
        }
        else {
            controlPanel.Select(null);
        }
        m_startObject = null;
        m_finishObject = null;
        if (m_previewBeam) {
            Destroy(m_previewBeam.gameObject);
            m_previewBeam = null;
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
            Transform targetModel = hit.transform;

            // controller rumble if on new object
            if (targetModel != m_lastObject) {
                m_lastObject = targetModel;
                SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse();
            }

            if (targetModel.parent == null) {
                endPos = hit.point;
            }
            else if (m_startObject.tag == "SourceFieldCell" && targetModel.parent.tag == "TargetFieldCell") {
                endPos = targetModel.position - 0.5f * targetModel.right * targetModel.lossyScale.x;
            }
            else if (m_startObject.tag == "TargetFieldCell" && targetModel.parent.tag == "SourceFieldCell") {
                endPos = targetModel.position + 0.5f * targetModel.right * targetModel.lossyScale.x;
            }
            else {
                endPos = hit.point;
            }
        }
        else {
            endPos = transform.position + m_startDistance * transform.forward;
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
