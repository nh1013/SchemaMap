using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Fit onto FieldCell prefab
public class FieldCell : MonoBehaviour
{
    public Transform m_boxMesh;
    public TextMesh m_fieldName;
    public TextMesh m_fieldType;

    public string m_fullName;       // full name set by table manager on creation
    public float m_scale = 0.01f;

    // Use this for initialization
    void Start() {
        transform.localScale = new Vector3(m_scale, m_scale, m_scale);
    }

    /// <summary>
    /// Get the lossy scale height of the boxMesh, relative to this cell
    /// </summary>
    public float GetHeight() {
        return m_scale * m_boxMesh.localScale.y;
    }

    /*
    /// <summary>
    /// [Deprecated] Get the full name of this cell
    /// </summary>
    public string GetFullName() {
        return m_fullName;
    }
    */
}
