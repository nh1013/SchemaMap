// Script modified from Oliver Robinson's "Mind Explorer VR" project, ModelManipulator.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManipulator : MonoBehaviour {
    public ControlPanel controlPanel;

    public Transform m_camera;
    public Transform m_controller;
    private SteamVR_TrackedController m_trackedController;

    public Transform m_sourceTable;
    public Transform m_targetTable;

    public Transform m_transformHost;
    public Transform m_sourceFake;
    public Transform m_targetFake;
    
    private Vector3 m_startingSourcePos;
    private Vector3 m_startingTargetPos;
    private Vector3 m_startingControllerPos;
    private Vector3 m_startingControllerForward;
    private Vector3 m_startingHostPos;
    private Quaternion m_hostControllerAngle;
    private float m_startingDistance;

    public float m_moveScale = 5.0f;
    private bool m_manipulationMode;

    // Use this for initialization
    void Start () {
        if (m_controller)
            m_trackedController = m_controller.GetComponent<SteamVR_TrackedController>();
        else
            Debug.Log("ERROR: Missing controller reference in ModelManipulator!");

        if (!m_trackedController)
            Debug.Log("ERROR: Couldn't retrieve vrTracked_controller components. Make sure they're attached to both controllers");

        if(m_trackedController) {
            m_trackedController.Gripped += new ClickedEventHandler(ControllerGrip);
            m_trackedController.Ungripped += new ClickedEventHandler(ControllerUngrip);
        }

    }

    private void ControllerGrip(object sender, ClickedEventArgs e) {
        if (controlPanel.SelectedSourceField) {
            m_sourceTable = controlPanel.SelectedSourceField.parent;
        }
        else {
            m_sourceTable = null;
        }
        if (controlPanel.SelectedTargetField) {
            m_targetTable = controlPanel.SelectedTargetField.parent;
        }
        else {
            m_targetTable = null;
        }
        SetGrip(true);
    }

    private void ControllerUngrip(object sender, ClickedEventArgs e) {
        SetGrip(false);
    }

    private void SetGrip(bool gripped) {
        if (!gripped) {
            m_manipulationMode = false;
            return;
        }

        // else, gripped == true
        m_manipulationMode = true;
        m_startingControllerPos = m_controller.position;
        m_startingControllerForward = m_controller.forward;

        if (m_sourceTable && m_targetTable) {
            // when two tables selected, use transform host to handle uniform position and rotate
            m_transformHost.position = Vector3.Lerp(m_sourceTable.position, m_targetTable.position, 0.5f);
            m_sourceFake.position = m_sourceTable.position;
            m_targetFake.position = m_targetTable.position;
        }
        else if (m_sourceTable) {
            m_transformHost.position = m_sourceTable.position;
            m_sourceFake.position = m_sourceTable.position;
        }
        else if (m_targetTable) {
            m_transformHost.position = m_targetTable.position;
            m_targetFake.position = m_targetTable.position;
        }
        else {
            // no tables selected
            return;
        }

        // look at controller without leaning
        m_transformHost.LookAt(new Vector3(
            m_camera.position.x,
            m_transformHost.position.y,
            m_camera.position.z
        ));
        m_startingDistance = Vector3.Distance(m_transformHost.position, m_startingControllerPos);
        m_hostControllerAngle.SetFromToRotation(m_controller.forward, m_transformHost.position - m_startingControllerPos);
    }
    
    // Update is called once per frame
    void Update () {

        // If we're not manipulating objects, return
        if (!m_manipulationMode) {
            return;
        }
        // if no object to manipulate, return
        if (!m_sourceTable && !m_targetTable) {
            return;
        }

        // move transform host to same distance against controller, same angle as initial selection
        //float angle = Vector3.Angle(m_startingControllerForward, m_controller.position - m_startingControllerPos);
        //float movePercentage = 1f - 0.01f * (m_startingDistance - Vector3.Distance(m_startingControllerPos, m_controller.position));
        //m_transformHost.position = m_controller.position +
        //    m_hostControllerAngle * m_controller.forward * (m_startingDistance - Mathf.Cos(angle * Mathf.Deg2Rad) * m_startingDistance);
        m_transformHost.position = m_controller.position + m_hostControllerAngle * m_controller.forward * m_startingDistance;
        // look at controller without leaning
        m_transformHost.LookAt(new Vector3(
            m_camera.position.x,
            m_transformHost.position.y,
            m_camera.position.z
        ));
        
        // copy back adjusted positions
        if (m_sourceTable) {
            m_sourceTable.position = m_sourceFake.position;
            m_sourceTable.rotation = m_sourceFake.rotation;
        }
        if (m_targetTable) {
            m_targetTable.position = m_targetFake.position;
            m_targetTable.rotation = m_targetFake.rotation;
        }
    }
}