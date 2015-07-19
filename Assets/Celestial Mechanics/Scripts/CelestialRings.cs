using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class CelestialRings : MonoBehaviour {
		[SerializeField] float innerRadius = 1f;
		[SerializeField] float outerRadius = 1.5f;
		[Range(3,50), SerializeField] int segments = 10;

		void Reset() {
			innerRadius = 1f;
			outerRadius = 1.5f;
			segments = 10;
		}

		void OnValidate() {
			if (innerRadius < 0) innerRadius = 0f;
			if (outerRadius < 0) outerRadius = 0f;
			Generate();
		}

		public void Generate() {
			MeshFilter filter = GetComponent<MeshFilter>();
			DestroyImmediate(filter.sharedMesh);
			filter.sharedMesh = GenerateRingMesh(innerRadius, outerRadius, segments);
		}

		public static Mesh GenerateRingMesh(float ri, float ro, int segments) {
			if (segments <= 0)
				throw new System.ArgumentOutOfRangeException("Number of segments cannot be less than or equal to 0");
			
			Mesh mesh = new Mesh();
			mesh.name = "Ring Mesh";
			
			Vector3[] vertices = new Vector3[segments*4];
			Vector2[] uvs = new Vector2[segments*4];
			int[] triangles = new int[segments*6];
			
			/*
			 * 	n+1-------n
			 * 	 |\       |
			 *   | \      |
			 * 	 |  \     |
			 * 	 |   \    |
			 * 	 |    \   |
			 * 	 |     \  |
			 * 	 |      \ |
			 * 	n+3------n+2
			 * 
			 *   n  = ro < i	->	uv(1,0)
			 *  n+1 = ro < i+1	->	uv(1,1)
			 *  n+2 = ri < i	->	uv(0,0)
			 *  n+3 = ri < i+1	->	uv(0,1)
			 */
			
			for (int i = 0; i < segments; i++) {
				float angle = i*2f*Mathf.PI/segments;
				float angle2 = (i+1)*2f*Mathf.PI/segments;
				
				vertices[4*i+0] = new Vector3(ro*Mathf.Cos(angle), 0f, ro*Mathf.Sin(angle));
				vertices[4*i+1] = new Vector3(ro*Mathf.Cos(angle2), 0f, ro*Mathf.Sin(angle2));
				vertices[4*i+2] = new Vector3(ri*Mathf.Cos(angle), 0f, ri*Mathf.Sin(angle));
				vertices[4*i+3] = new Vector3(ri*Mathf.Cos(angle2), 0f, ri*Mathf.Sin(angle2));
				
				uvs[4*i+0] = new Vector2(1f,0f);
				uvs[4*i+1] = new Vector2(1f,1f);
				uvs[4*i+2] = new Vector2(0f,0f);
				uvs[4*i+3] = new Vector3(0f,1f);
				
				//tri 1
				triangles[6*i+0] = 4*i+2;
				triangles[6*i+1] = 4*i+1;
				triangles[6*i+2] = 4*i+0;
				//tri 2
				triangles[6*i+3] = 4*i+1;
				triangles[6*i+4] = 4*i+2;
				triangles[6*i+5] = 4*i+3;
			}
			
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			
			mesh.Optimize();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			
			return mesh;
		}
	}
}