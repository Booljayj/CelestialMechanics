using UnityEngine;
using System.Collections;
using System;

namespace CelestialMechanics {
	[RequireComponent(typeof(CelestialOrbit))]
	public class CelestialOrbitDecay : MonoBehaviour, ISimulation {
		CelestialOrbit orbit;

		public double orbitDecay = 0f; //[units/s]
		public float apsidalPrecession = 0f; //[deg/s]

		public double semiMajorAxisOriginal {get; private set;}
		public float argumentOriginal {get; private set;}
		public double periodOriginal {get; private set;}

		void Awake() {
			orbit = GetComponent<CelestialOrbit>();

			semiMajorAxisOriginal = orbit.periapsis;
			argumentOriginal = orbit.argument;
			periodOriginal = orbit.period;
		}

		void Update() {
			if (orbit.simulate) UpdateSimulation();
		}

		public void StartSimulation() {orbit.simulate = true;}
		public void StopSimulation() {orbit.simulate = false;}

		public void ResetSimulation() {
			orbit.periapsis = semiMajorAxisOriginal;
			orbit.argument = argumentOriginal;
			orbit.period = periodOriginal;
		}

		public void UpdateSimulation() {
			if (orbitDecay != 0) {
				orbit.periapsis += orbitDecay * Time.deltaTime * orbit.timeScale;

				if (orbit.periapsis <= 0) {
					//behaviour beyond this point is undefined, best not upset the Kraken
					ResetSimulation();
					StopSimulation();
					orbit.StopSimulation();

				} else {
					orbit.period = Math.Sqrt( (periodOriginal*periodOriginal) *
					                          (orbit.periapsis*orbit.periapsis*orbit.periapsis) /
					                          (semiMajorAxisOriginal*semiMajorAxisOriginal*semiMajorAxisOriginal) );
				}
			}

			if (apsidalPrecession != 0) {
				orbit.argument = Kepler.WrapAngle(orbit.argument - apsidalPrecession * Time.deltaTime * (float)orbit.timeScale, -180, 180);
			}
		}
	}
}