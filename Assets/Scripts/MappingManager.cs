using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MappingManager : MonoBehaviour
{
    public Transform BeamPrefab;

    public Transform SourceManager;
    public Transform TargetManager;

    public bool debugMode = false;
    public List<Transform> m_BeamList;

    // Use this for initialization
    void Start() {

    }

    /// <summary>
    /// In-game addition of a beam, specifying fields via strings
    /// </summary>
    /// <param name="sTableName">Source schema's table name.</param>
    /// <param name="sFieldName">Source table's field name.</param>
    /// <param name="tTableName">Target schema's table name.</param>
    /// <param name="tFieldName">Target table's field name.</param>
    /// <param name="confidence">Confidence value of connection.</param>
    /// <returns>True if load was successful, false if load failed and aborted.</returns>
    public bool AddBeam(string sTableName, string sFieldName, string tTableName, string tFieldName, float confidence = 1.0f) {
        // check if the fields exist
        Transform sField, tField;
        Transform sTable = SourceManager.Find(sTableName);
        if (sTable == null) {
            Debug.Log("source table not found: " + sTableName);
            return false;
        }
        if (sFieldName.Length == 0) {
            sField = sTable.GetComponent<TableManager>().m_titleCell;
        }
        else {
            sField = sTable.Find(sFieldName);
            if (sField == null) {
                Debug.Log("source field not found: " + sFieldName);
                return false;
            }
        }

        Transform tTable = TargetManager.Find(tTableName);
        if (tTable == null) {
            Debug.Log("target table not found: " + tTableName);
            return false;
        }
        if (tFieldName.Length == 0) {
            tField = tTable.GetComponent<TableManager>().m_titleCell;
        }
        else {
            tField = tTable.Find(tFieldName);
            if (tField == null) {
                Debug.Log("target field not found: " + tFieldName);
                return false;
            }
        }

        // pass over to overloaded function to make sure duplicate beams are not created
        return AddBeam(sField, tField, confidence);
    }

    /// <summary>
    /// In-game addition of a beam, via selecting two fields
    /// </summary>
    /// <param name="sourceField">Transform of object from source side schema.</param>
    /// <param name="targetField">Transform of object from target side schema.</param>
    public Transform AddBeam(Transform sourceField, Transform targetField, float confidence = 1.0f) {
        // should check if beam already exists
        string sourceName = sourceField.GetComponent<FieldCell>().m_fullName;
        string targetName = targetField.GetComponent<FieldCell>().m_fullName;
        foreach (Transform beam in m_BeamList) {
            string beamSourceName = beam.GetComponent<MappingBeam>().m_SourceField.GetComponent<FieldCell>().m_fullName;
            if (sourceName != beamSourceName) {
                continue;
            }
            string beamTargetName = beam.GetComponent<MappingBeam>().m_TargetField.GetComponent<FieldCell>().m_fullName;
            if (targetName == beamTargetName) {
                Debug.Log("Warning: attempted to create beam that already exists, between " + beamSourceName + " and " + beamTargetName);
                return beam;
            }
        }

        // add the beam
        Transform newBeam = Instantiate(BeamPrefab, transform);
        m_BeamList.Add(newBeam);
        newBeam.GetComponent<MappingBeam>().SetValues(sourceField, targetField, confidence);
        return newBeam;
    }

    /// <summary>
    /// Find a beam, via the beam's end nodes
    /// </summary>
    /// <param name="sourceField">Start node of beam to be found.</param>
    /// <param name="targetField">End node of beam to be found.</param>
    public Transform FindBeam(Transform sourceField, Transform targetField) {
        string sourceName = sourceField.GetComponent<FieldCell>().m_fullName;
        string targetName = targetField.GetComponent<FieldCell>().m_fullName;
        foreach (Transform beam in m_BeamList) {
            string beamSourceName = beam.GetComponent<MappingBeam>().m_SourceField.GetComponent<FieldCell>().m_fullName;
            if (sourceName != beamSourceName) {
                continue;
            }
            string beamTargetName = beam.GetComponent<MappingBeam>().m_TargetField.GetComponent<FieldCell>().m_fullName;
            if (targetName != beamTargetName) {
                continue;
            }
            // beam matches target for removal
            if (debugMode) {
                Debug.Log("Beam found between " + sourceName + " and " + targetName);
            }
            return beam;
        }
        // no matches
        if (debugMode) {
            Debug.Log("No beam found between " + sourceName + " and " + targetName);
        }
        return null;
    }

    /// <summary>
    /// In-game removal of a beam, via selecting the beam's end nodes
    /// </summary>
    /// <remarks>
    /// With proper set up, this will never be called
    /// </remarks>
    /// <param name="sourceField">Start node of beam to be removed.</param>
    /// <param name="targetField">End node of beam to be removed.</param>
    public bool RemoveBeam(Transform sourceField, Transform targetField) {
        Transform beam = FindBeam(sourceField, targetField);
        if (beam != null) {
            Destroy(beam);
            return true;
        }
        // no matches
        Debug.Log("Error: could not locate beam, deletion failed");
        return false;
    }

    /// <summary>
    /// In-game removal of a beam, via selecting the beam directly
    /// </summary>
    /// <param name="beam">Object to be removed.</param>
    public bool RemoveBeam(Transform beam) {
        // check if item is a beam
        if (!m_BeamList.Contains(beam)) {
            Debug.Log("Error: target object for deletion is not a beam, name: " + beam.name);
            return false;
        }
        // remove this beam from the list
        m_BeamList.Remove(beam);
        Destroy(beam);
        return true;
    }
    
    /// <summary>
    /// Remove all beams
    /// </summary>
    public void ClearBeams() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
        m_BeamList.Clear();
    }
}
