using UnityEngine;
using System;
using System.Collections;

namespace CelestialMechanics {
	[RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
	public class CelestialOrbitPath : MonoBehaviour {
		const double Deg2Rad = Math.PI/180.0;
		
		public CelestialOrbit orbit;
		[Range(3,200)] public int segments = 50;

		LineRenderer line;

		void Awake() {
			line = GetComponent<LineRenderer>();

			if (orbit == null) {
				orbit = GetComponent<CelestialOrbit>();
			}
		}

		void OnEnable() {
			if (orbit == null) {
				line.SetVertexCount(0);

			} else {
				double step, lower, upper, r;
				Matrix4x4 matrix = (transform.parent)? transform.parent.localToWorldMatrix : Matrix4x4.identity;

				line.SetVertexCount(segments+1);

				upper = Kepler.ComputeTrueAnomaly(Kepler.ComputeEccentricAnomaly(orbit.limits.x*Deg2Rad, orbit.eccentricity), orbit.eccentricity);
				lower = Kepler.ComputeTrueAnomaly(Kepler.ComputeEccentricAnomaly(orbit.limits.y*Deg2Rad, orbit.eccentricity), orbit.eccentricity);
				step = (upper - lower)/segments;

				for (int i = 0; i < segments+1; i++) {
					r = Kepler.ComputeRadius(orbit.semiLatusRectum, orbit.eccentricity, lower + step*i);
					line.SetPosition(i, matrix.MultiplyPoint(orbit.orientation * Kepler.ComputePosition(r, lower + step*i)));
				}
			}
		}

		#if UNITY_EDITOR
		void Update() {
			OnEnable();
		}
		#endif
	}
}