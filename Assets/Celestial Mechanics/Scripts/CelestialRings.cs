using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	[RequireComponent(typeof(MeshFilter))]
	public class CelestialRings : MonoBehaviour {
		[SerializeField] float innerRadius = 1f;
		[SerializeField] float outerRadius = 1.5f;
		[Range(3,50), SerializeField] int segments = 10;

		void OnValidate() {
			if (innerRadius < 0) innerRadius *= -1f;
			if (outerRadius < 0) outerRadius *= -1f;
			Generate();
		}

		public void Generate() {
			MeshFilter filter = GetComponent<MeshFilter>();
			DestroyImmediate(filter.sharedMesh);
			filter.sharedMesh = Kepler.GenerateRingMesh(innerRadius, outerRadius, segments);
		}
	}
}