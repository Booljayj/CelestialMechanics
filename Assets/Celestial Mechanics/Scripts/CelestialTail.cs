using UnityEngine;
using System.Collections;

public class CelestialTail : MonoBehaviour {
	public void SetAnomaly(double anomaly) {
		transform.localRotation = Quaternion.Euler(0f, -(float)(anomaly)*Mathf.Rad2Deg, 0f);
	}
}
