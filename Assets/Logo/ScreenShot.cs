using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour {
	[UnityEngine.Serialization.FormerlySerializedAs("name")]
	public string fName;

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			ScreenCapture.CaptureScreenshot(Application.dataPath + "/"+fName+".png", 2);
			Debug.Log("Screenshot taken");
		}
	}
}