using UnityEngine;
using System.Collections;
using System.IO;

public class TriggerPhoto : MonoBehaviour
{
    private SteamVR_TrackedController m_trackedController;

    void Awake() {
        m_trackedController = GetComponent<SteamVR_TrackedController>();
        m_trackedController.TriggerClicked += new ClickedEventHandler(TakeScreenshot);
    }

    private void TakeScreenshot(object sender, ClickedEventArgs e) {
        // find unused filename
        int count = 0;
        string path = "Screenshots/Screenshot.png";
        while (File.Exists(path)) {
            count++;
            path = "Screenshots/Screenshot" + count + ".png";
        }
        ScreenCapture.CaptureScreenshot(path);
    }
}
