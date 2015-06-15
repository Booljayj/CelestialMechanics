using UnityEngine;
using System.Collections;

public class RadialFlyCam : MonoBehaviour {
	public float maxRadius = 30f;
	public float sensitivity = 10f;
	public float speed = 10f;

	Vector3 facing;
	float rotationX;
	float rotationY;
	Vector3 Hposition;
	
	void Update () {
		rotationX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
		rotationY = Mathf.Clamp (rotationY, -90, 90);
		
		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		facing = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

		transform.position += facing * speed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime;

		Hposition = new Vector3(transform.position.x, 0, transform.position.y);
		if (Vector3.Distance(Vector3.zero, Hposition) > maxRadius) {
			Hposition = Hposition.normalized * maxRadius;
			transform.position = new Vector3(Hposition.x, transform.position.y, Hposition.z);
		}
	}
}
