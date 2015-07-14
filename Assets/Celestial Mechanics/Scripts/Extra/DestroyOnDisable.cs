using UnityEngine;
using System.Collections;

public class DestroyOnDisable : MonoBehaviour {
	public void OnDisable() {
		Destroy(gameObject);
	}
}
