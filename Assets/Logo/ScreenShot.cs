using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour {
	public string name;

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Application.CaptureScreenshot(Application.dataPath + "/"+name+".png", 2);
			Debug.Log("Screenshot taken");
		}
	}
}