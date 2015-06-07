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
				double step, lower, E, v, r;
				line.SetVertexCount(segments+1);

				step = (orbit.limits.y - orbit.limits.x)*Deg2Rad/segments;
				lower = orbit.limits.x*Deg2Rad;

				for (int i = 0; i < segments+1; i++) {
					E = Kepler.ComputeEccentricAnomaly(lower+step*i, orbit.eccentricity);
					v = Kepler.ComputeTrueAnomaly(E, orbit.eccentricity);
					r = Kepler.ComputeRadius(orbit.semiLatusRectum, orbit.eccentricity, v);
					if (orbit.transform.parent) {
						line.SetPosition(i, orbit.orientation * orbit.transform.parent.TransformPoint(Kepler.ComputePosition(r, v)));
					} else {
						line.SetPosition(i, orbit.orientation * Kepler.ComputePosition(r, v));
					}
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