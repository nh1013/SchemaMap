using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ControlPanel : MonoBehaviour
{
    public FileManager fileManager;
    public MappingManager mapManager;

    // menus
    public Transform m_mainMenu;
    public Transform m_filesMenu;
    public Transform m_editMenu;

    // import/export menu objects
    public Dropdown m_sourceSchemaDropdown;
    public Dropdown m_targetSchemaDropdown;
    public Dropdown m_mappingDropdown;

    // edit menu objects
    public Text m_sourceFieldText;
    public Text m_targetFieldText;
    public Text m_beamConfidenceText;
    public Button m_linkButton;
    public Button m_deleteButton;

    // selected items
    public Material m_selection_AURA_MTL;
    private Transform m_selectedSourceField;
    private Transform m_selectedTargetField;
    private Transform m_selectedMappingBeam;

    public Transform SelectedSourceField {
        get {
            return m_selectedSourceField;
        }
    }

    public Transform SelectedTargetField {
        get {
            return m_selectedTargetField;
        }
    }

    // Use this for initialization
    void Start() {
        ShowMenu(m_mainMenu);
    }

    // navigation function
    /// <summary>
    /// Show the selected menu, by deactivating all others
    /// </summary>
    /// <param name="menu">The menu to be shown.</param>
    public void ShowMenu (Transform menu) {
        if (menu.tag != "Menu") {
            Debug.Log("Error: object not a menu: " + menu.name);
            return;
        }
        m_mainMenu.gameObject.SetActive(false);
        m_filesMenu.gameObject.SetActive(false);
        m_editMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        return;
    }

    // file menu functions
    /// <summary>
    /// Import to the source schema manager the schema selected by the control panel
    /// </summary>
    public void ImportSource() {
        string selection = m_sourceSchemaDropdown.captionText.text;
        if (selection == "Select schema") {
            Debug.Log("No database selected");
            return;
        }
        fileManager.ImportSchema(selection, true);
    }

    /// <summary>
    /// Import to the target schema manager the schema selected by the control panel
    /// </summary>
    public void ImportTarget() {
        string selection = m_targetSchemaDropdown.captionText.text;
        if (selection == "Select schema") {
            Debug.Log("No database selected");
            return;
        }
        fileManager.ImportSchema(selection, false);
    }

    /// <summary>
    /// Import the mapping selected by the control panel
    /// </summary>
    public void ImportMapping() {
        string selection = m_mappingDropdown.captionText.text;
        if (selection == "Select mapping") {
            Debug.Log("No database selected");
            return;
        }
        fileManager.ImportMapping(selection);
    }

    /// <summary>
    /// Export the current mapping
    /// </summary>
    public void ExportMapping() {
        fileManager.ExportMapping();
    }

    // edit menu functions
    /// <summary>
    /// Generate a highlight outline for the field cell or mapping beam
    /// </summary>
    /// <param name="item">The object being highlighted.</param>
    public void Highlight(Transform item) {
        if (item == null) {
            return;
        }
        if (item.tag != "SourceFieldCell" && item.tag != "TargetFieldCell" && item.tag != "MappingBeam") {
            Debug.Log("Error: received object that should not be highlighted: " + item.name);
            return;
        }

        Transform model = null;
        for (int i = item.childCount - 1; i >= 0; i--) {
            if (item.GetChild(i).name == "Model") {
                model = item.GetChild(i);
                break;
            }
        }
        if (model == null) {
            Debug.Log("Error: received object has no child named Model");
            return;
        }
        // ignore existing children: Gameobject.destroy is not immediate
        //if (model.childCount != 0) {
        //    Debug.Log("Warning: received object already has children, presumed highlighter");
        //    return;
        //}

        // create selection outline object as a child of highlighted object
        GameObject outline = null;
        if (item.tag == "SourceFieldCell" || item.tag == "TargetFieldCell") {
            outline = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(outline.GetComponent<BoxCollider>());
        }
        else {
            outline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(outline.GetComponent<CapsuleCollider>());
        }
        outline.transform.SetParent(model);
        outline.transform.localScale = Vector3.one;
        outline.transform.localPosition = Vector3.zero;
        outline.transform.localRotation = Quaternion.identity;
        outline.GetComponent<MeshRenderer>().material = m_selection_AURA_MTL;
    }
    
    /// <summary>
    /// Find and delete the highlight outline for the field cell or mapping beam
    /// </summary>
    /// <param name="item">The object being de-highlighted.</param>
    public void DeHighlight(Transform item) {
        if (item == null) {
            return;
        }
        if (item.gameObject.tag != "SourceFieldCell" && item.gameObject.tag != "TargetFieldCell" && item.gameObject.tag != "MappingBeam") {
            Debug.Log("Error: received object that should not be de-highlighted: " + item.name);
            return;
        }
        Transform model = null;
        for (int i = item.childCount - 1; i >= 0; i--) {
            if (item.GetChild(i).name == "Model") {
                model = item.GetChild(i);
                break;
            }
        }
        if (model == null) {
            Debug.Log("Error: received object has no child named Model");
            return;
        }
        if (model.childCount == 0) {
            Debug.Log("Warning: received object model has no children, presumed not highlighted");
        }

        for (int i = model.childCount - 1; i >= 0; i--) {
            Destroy(model.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Select/unselect the item, and add/remove others as suitable
    /// </summary>
    /// <param name="item">The object being selected.</param>
    public void Select(Transform item) {
        DeHighlight(m_selectedSourceField);
        DeHighlight(m_selectedTargetField);
        DeHighlight(m_selectedMappingBeam);
        if (!item) {
            m_selectedSourceField = null;
            m_selectedTargetField = null;
            m_selectedMappingBeam = null;
        }
        else if (item.gameObject.tag == "SourceFieldCell") {
            if (m_selectedSourceField == item) {
                m_selectedSourceField = null;
            }
            else {
                m_selectedSourceField = item;
            }
            m_selectedMappingBeam = mapManager.FindBeam(m_selectedSourceField, m_selectedTargetField);
        }
        else if (item.gameObject.tag == "TargetFieldCell") {
            if (m_selectedTargetField == item) {
                m_selectedTargetField = null;
            }
            else {
                m_selectedTargetField = item;
            }
            m_selectedMappingBeam = mapManager.FindBeam(m_selectedSourceField, m_selectedTargetField);
        }
        else if (item.gameObject.tag == "MappingBeam") {
            if (m_selectedMappingBeam == item) {
                m_selectedMappingBeam = null;
                m_selectedSourceField = null;
                m_selectedTargetField = null;
            }
            else {
                m_selectedMappingBeam = item;
                m_selectedSourceField = m_selectedMappingBeam.GetComponent<MappingBeam>().m_SourceField;
                m_selectedTargetField = m_selectedMappingBeam.GetComponent<MappingBeam>().m_TargetField;
            }
        }
        Highlight(m_selectedSourceField);
        Highlight(m_selectedTargetField);
        Highlight(m_selectedMappingBeam);
        UpdateEditMenu();
    }

    /// <summary>
    /// De-select the item, if applicable
    /// </summary>
    /// <param name="item">The object being de-selected.</param>
    public void DeSelect(Transform item) {
        DeHighlight(m_selectedSourceField);
        DeHighlight(m_selectedTargetField);
        DeHighlight(m_selectedMappingBeam);
        if (item.gameObject.tag == "SourceFieldCell") {
            if (m_selectedSourceField != item) {
                Debug.Log("Error: attempted to deselect something not selected");
            }
            else {
                m_selectedSourceField = null;
                m_selectedMappingBeam = null;
            }
        }
        else if (item.gameObject.tag == "TargetFieldCell") {
            if (m_selectedTargetField != item) {
                Debug.Log("Error: attempted to deselect something not selected");
            }
            else {
                m_selectedTargetField = null;
                m_selectedMappingBeam = null;
            }
        }
        else if (item.gameObject.tag == "MappingBeam") {
            if (m_selectedMappingBeam != item) {
                Debug.Log("Error: attempted to deselect something not selected");
            }
            else {
                m_selectedSourceField = null;
                m_selectedTargetField = null;
                m_selectedMappingBeam = null;
            }
        }
        Highlight(m_selectedSourceField);
        Highlight(m_selectedTargetField);
        Highlight(m_selectedMappingBeam);
        UpdateEditMenu();
    }

    /// <summary>
    /// Update the entries in edit menu
    /// </summary>
    public void UpdateEditMenu() {
        m_sourceFieldText.text = (m_selectedSourceField) ? m_selectedSourceField.GetComponent<FieldCell>().m_fullName : "not selected";
        m_targetFieldText.text = (m_selectedTargetField) ? m_selectedTargetField.GetComponent<FieldCell>().m_fullName : "not selected";
        m_beamConfidenceText.text = (m_selectedMappingBeam) ? m_selectedMappingBeam.GetComponent<MappingBeam>().m_confidence.ToString() : "none";

        m_linkButton.gameObject.SetActive(false);
        m_deleteButton.gameObject.SetActive(false);
        if (m_selectedSourceField && m_selectedTargetField) {
            if (m_selectedMappingBeam) {
                m_deleteButton.gameObject.SetActive(true);
            }
            else {
                m_linkButton.gameObject.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// Link two selected fields together
    /// </summary>
    public void LinkFields() {
        if (!m_selectedSourceField || !m_selectedTargetField) {
            Debug.Log("Error: requires two fields selected");
            return;
        }
        Select(mapManager.AddBeam(m_selectedSourceField, m_selectedTargetField));
    }

    /// <summary>
    /// Delete the selected mapping beam
    /// </summary>
    public void DeleteLink() {
        if (!m_selectedMappingBeam) {
            Debug.Log("Error: no object selected");
            return;
        }
        // [shader] if applicable, remove shader of m_selectedMappingBeam
        mapManager.RemoveBeam(m_selectedMappingBeam);
        m_selectedMappingBeam = null;
        UpdateEditMenu();
    }
}
