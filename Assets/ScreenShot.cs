using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour {
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Application.CaptureScreenshot(Application.dataPath + "/Screenshot.png", 2);
			Debug.Log("Screenshot taken");
		}
	}
}