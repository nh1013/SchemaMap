using UnityEngine;
using System.Collections;
using System.IO;

public class TriggerPhoto : MonoBehaviour
{
    private SteamVR_TrackedController trackedObj;

    private bool m_canTake = true; // primitive semaphore

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedController>();
        m_canTake = true;
    }

    private void TakeScreenshot() {
        if (!m_canTake) {
            return;
        }

        m_canTake = false;
        // find unused filename
        int count = 0;
        string path = "Screenshots/Screenshot.png";
        while (File.Exists(path)) {
            count++;
            path = "Screenshots/Screenshot" + count + ".png";
        }
        ScreenCapture.CaptureScreenshot(path);

        new WaitForSeconds(1);
        m_canTake = true;
    }

    // Update is called once per frame
    void Update() {
        // take a screenshot when the controller's trigger is pressed
        if (trackedObj.triggerPressed) {
            TakeScreenshot();
        }
    }
}
