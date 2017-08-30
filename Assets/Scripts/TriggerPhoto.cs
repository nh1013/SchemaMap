using UnityEngine;
using System.Collections;
using System.IO;

public class TriggerPhoto : MonoBehaviour
{
    private SteamVR_TrackedController trackedObj;

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedController>();
    }

    // Update is called once per frame
    void Update() {
        // take a screenshot when the controller's trigger is pressed
        if (trackedObj.triggerPressed) {
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
}
