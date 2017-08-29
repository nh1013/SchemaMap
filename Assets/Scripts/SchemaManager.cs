using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchemaManager : MonoBehaviour
{
    //public enum SCHEMA_TYPE { SOURCE, TARGET};

    public Transform TablePrefab;
    public Transform SourceFieldCellPrefab;
    //public Transform TargetFieldCellPrefab;

    public List<Transform> m_tableList;
    
    public bool debugMode = false;
    public bool m_isSource = true;      // true for source manager, false for target manager
    public string m_schemaName;
    public string m_databaseName = "";
    private float m_spacing = 0.5f;
    private float m_bottomSpace = 0;

	// Use this for initialization
	void Start () {
		
	}

    /*
    /// <summary>
    /// [Deprecated] Load a schema and create tables
    /// </summary>
    /// <param name="name">Name of the schema.</param>
    /// <param name="fields">List of tables to be created and managed.</param>
    private void LoadSchema(string name, List<Table> table_list ) {
        ClearSchema();
        m_schemaName = name;
        foreach (Table table in table_list) {
            CreateTable(table.name, table.fields);
        }
    }
    */

    /// <summary>
    /// Clean up Schema, removing all children (tables)
    /// </summary>
    public void ClearSchema() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
        m_tableList.Clear();
        m_schemaName = "";
        m_bottomSpace = 0;
    }

    /// <summary>
    /// Create a new table
    /// </summary>
    /// <param name="name">Name of the table.</param>
    /// <param name="fields">Pairs of strings, indicating each field's name and type.</param>
    public void CreateTable(string name, List<StrPair> fields) {
        Transform table = Instantiate(TablePrefab, transform);
        if (m_isSource) {
            table.GetComponent<TableManager>().FieldCellPrefab = SourceFieldCellPrefab;
        }
        table.localPosition = new Vector3(0.0f, m_bottomSpace, 0.0f);
        table.GetComponent<TableManager>().SetName(name);
        table.GetComponent<TableManager>().SetFields(fields);
        m_tableList.Add(table);
        m_bottomSpace -= table.GetComponent<TableManager>().GetHeight() + m_spacing;
    }
}
