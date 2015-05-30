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

		public double semiMajorAxisCurrent {get; private set;}
		public float argumentCurrent {get; private set;}
		public double periodCurrent {get; private set;}

		void Awake() {
			orbit = GetComponent<CelestialOrbit>();

			semiMajorAxisOriginal = orbit.semiMajorAxis;
			argumentOriginal = orbit.argument;
			periodOriginal = orbit.period;

			ResetSimulation();
		}

		void Update() {
			if (orbit.simulate) UpdateSimulation();
		}

		public void StartSimulation() {orbit.simulate = true;}
		public void StopSimulation() {orbit.simulate = false;}

		public void ResetSimulation() {
			semiMajorAxisCurrent = semiMajorAxisOriginal;
			orbit.semiMajorAxis = semiMajorAxisOriginal;

			argumentCurrent = argumentOriginal;
			orbit.argument = argumentOriginal;

			periodCurrent = periodOriginal;
			orbit.period = periodOriginal;
		}

		public void UpdateSimulation() {
			if (orbitDecay != 0) {
				semiMajorAxisCurrent += orbitDecay * Time.deltaTime * orbit.timeScale;
				periodCurrent = Math.Sqrt( (periodOriginal*periodOriginal) *
				                           (semiMajorAxisCurrent*semiMajorAxisCurrent*semiMajorAxisCurrent) /
				                           (semiMajorAxisOriginal*semiMajorAxisOriginal*semiMajorAxisOriginal) );

				if (semiMajorAxisCurrent <= 0) {
					//behaviour beyond this point is undefined, best not upset the Kraken
					ResetSimulation();
					StopSimulation();
					orbit.StopSimulation();

				} else {
					orbit.semiMajorAxis = semiMajorAxisCurrent;
					orbit.period = periodCurrent;
				}
			}

			if (apsidalPrecession != 0) {
				argumentCurrent = Kepler.WrapAngle(argumentCurrent - 
				                                   apsidalPrecession * Time.deltaTime * (float)orbit.timeScale, -180, 180);

				orbit.argument = argumentCurrent;
			}
		}
	}
}