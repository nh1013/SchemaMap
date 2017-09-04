using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Text m_messageText;
    public float m_destructionTime = 5.0f;

    // Use this for initialization
    void Start() {
        Destroy(this.gameObject, m_destructionTime);
    }

    // Update is called once per frame
    void Update() {
        transform.position += new Vector3(0, 0.001f, 0);
    }
}
