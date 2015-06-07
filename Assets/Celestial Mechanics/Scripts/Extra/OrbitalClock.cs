using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	public class OrbitalClock : MonoBehaviour {
		public CelestialOrbit seconds;
		public CelestialOrbit minutes;
		public CelestialOrbit hours;
		public CelestialOrbit days;

		void Start () {
			System.DateTime time = System.DateTime.Now;
			seconds.meanAnomaly = (float)time.Millisecond/1000f*360f;
			minutes.meanAnomaly = (float)time.Second/60f*360f;
			hours.meanAnomaly = (float)time.Minute/60f*360f;
			days.meanAnomaly = (float)time.Hour/24f*720f;

			seconds.ResetSimulation();
			minutes.ResetSimulation();
			hours.ResetSimulation();
			days.ResetSimulation();
		}
	}
}