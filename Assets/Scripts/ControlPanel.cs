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
    private Transform m_selectedSourceField;
    private Transform m_selectedTargetField;
    private Transform m_selectedMappingBeam;

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
    /// Sort the selected item into its slot, 
    /// and add/remove others as suitable
    /// </summary>
    /// <param name="item">The object being selected.</param>
    public void Select(Transform item) {
        // [shader] unshade m_selectedSourceField, m_selectedTargetField, and m_selectedMappingBeam
        if (item.gameObject.tag == "SourceFieldCell") {
            m_selectedSourceField = item;
            m_selectedMappingBeam = mapManager.FindBeam(m_selectedSourceField, m_selectedTargetField);
        }
        else if (item.gameObject.tag == "TargetFieldCell") {
            m_selectedTargetField = item;
            m_selectedMappingBeam = mapManager.FindBeam(m_selectedSourceField, m_selectedTargetField);
        }
        else if (item.gameObject.tag == "MappingBeam") {
            m_selectedMappingBeam = item;
            m_selectedSourceField = m_selectedMappingBeam.GetComponent<MappingBeam>().m_SourceField;
            m_selectedTargetField = m_selectedMappingBeam.GetComponent<MappingBeam>().m_TargetField;
        }
        // [shader] shade m_selectedSourceField, m_selectedTargetField, and m_selectedMappingBeam
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
