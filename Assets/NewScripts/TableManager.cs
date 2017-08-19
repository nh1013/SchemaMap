using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TableManager : MonoBehaviour {
    public GameObject FieldCellPrefab;      // used to store cells
    
    public List<RectTransform> m_fields;    // list of field objects
    public Text m_titleCell;                // Text of object storing the title
    private Transform table;
    
    
    // Use this for initialization
    void Start()
    {

    }

    void Select() {
        // turn on selection in Model Manipulator
    }

    void UnSelect() {
        // turn off selection in Model Manipulator
    }

    void SetName(string name) {
        m_titleCell.text = name;
    }

    void SetFields(List<StrPair> fields) {
        foreach (StrPair pair in fields) {
            GameObject cell = Instantiate(FieldCellPrefab, table);
            Transform cellTransform = cell.transform;
            cellTransform.Find("Field").GetComponent<Text>().text = pair.field;
            cellTransform.Find("Type").GetComponent<Text>().text = pair.type;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
