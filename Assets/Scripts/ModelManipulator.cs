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
    private int m_controllerIndex = -1;
    private SteamVR_Controller.Device m_controllerDevice;

    private Transform m_sourceTable;
    private Transform m_targetTable;

    public Transform m_transformHost;
    public Transform m_sourceFake;
    public Transform m_targetFake;
    
    private Vector3 m_startingControllerPos;
    private Quaternion m_hostControllerAngle;
    private float m_modelDistance;

    public float m_moveScale = 5.0f;
    public bool m_manipulationMode;

    // Use this for initialization
    void Start () {
        if (m_controller)
            m_trackedController = m_controller.GetComponent<SteamVR_TrackedController>();
        else
            Debug.Log("ERROR: Missing controller reference in ModelManipulator!");

        if (!m_trackedController)
            Debug.Log("ERROR: Couldn't retrieve vrTracked_controller components.");

        if(m_trackedController) {
            m_trackedController.Gripped += new ClickedEventHandler(ControllerGrip);
            m_trackedController.Ungripped += new ClickedEventHandler(ControllerUngrip);
        }
        m_controllerIndex = (int) m_controller.GetComponent<SteamVR_TrackedObject>().index;
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

        if (m_controllerIndex == -1) {
            m_controllerIndex = (int)m_controller.GetComponent<SteamVR_TrackedObject>().index;
            m_controllerDevice = SteamVR_Controller.Input(m_controllerIndex);
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

        if (m_sourceTable && m_targetTable) {
            // when two tables selected, use transform host to handle uniform position and rotate
            m_transformHost.position = Vector3.Lerp(m_sourceTable.position, m_targetTable.position, 0.5f);
            m_transformHost.rotation = Quaternion.LookRotation(Vector3.Cross(m_targetTable.position - m_sourceTable.position, Vector3.down));
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
        m_modelDistance = Vector3.Distance(m_transformHost.position, m_startingControllerPos);
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

        if (m_trackedController.padPressed){
            if (m_controllerDevice.GetAxis().y > 0) {
                m_modelDistance += 0.1f;
            }
            else {
                m_modelDistance -= 0.1f;
            }
        }

        // move transform host to same distance against controller, same angle as initial selection
        m_transformHost.position = m_controller.position + m_hostControllerAngle * m_controller.forward * m_modelDistance;
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