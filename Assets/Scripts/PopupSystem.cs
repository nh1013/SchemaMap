using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupSystem : MonoBehaviour
{
    public Transform PopupPrefab;

    public Transform m_camera;
    
    public void DisplayMessage(string msg) {
        Transform popup = Instantiate(PopupPrefab, transform);
        popup.position = m_camera.position + 0.5f * m_camera.forward - 0.3f * m_camera.up + 0.3f * m_camera.right;
        popup.LookAt(new Vector3(
            m_camera.position.x,
            popup.position.y,
            m_camera.position.z
        ));
        popup.GetComponent<Popup>().m_messageText.text = msg;
    }
}
