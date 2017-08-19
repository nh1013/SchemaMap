using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct StrPair {
    public string field;
    public string type;

    public StrPair (string _field, string _type) {
        field = _field;
        type = _type;
    }
}

public struct Table {
    public string name;
    public List<StrPair> fields;

    public Table(string _name, List<StrPair> _fields) {
        name = _name;
        fields = _fields;
    }
}

public class FileManager : MonoBehaviour {

    public SchemaManager SourceManager;
    public SchemaManager TargetManager;

    // Use this for initialization
    void Start () {
        RefreshFiles("Schemas");
        RefreshFiles("Mappings");
    }

    void RefreshFiles (string folder) {
        DirectoryInfo dir = new DirectoryInfo(folder + "/");
        List<string> fileNames = new List<string> { };
        FileInfo[] info;
        if (folder == "Schemas") {
            info = dir.GetFiles("*.sql");
        }
        else if (folder == "Mappings") {
            info = dir.GetFiles("*.txt");
        }
        else {
            Debug.Log("RefreshFiles: folder not recognised: " + folder);
            return;
        }

        foreach (FileInfo file in info)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(file.ToString()));
        }
        // put list of fileNames onto control panel
        
    }

    void ImportSchema(string fileName, bool SchemaType) {
        // read file
        // parse through file to get 
        // database name
        // tables: table name, and pairs of fields/field types
        // ignore constraints
        // recognise end of table
        // recognise end of file

        // depending on if SchemaType is SOURCE or TARGET,
        // load schema in correct schema manager
    }
    
    void ImportMapping() {

    }

    void ExportMapping() {

    }

    // Update is called once per frame
    void Update () {
		
	}
}
