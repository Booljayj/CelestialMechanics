using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	public interface ISimulation {
		void StartSimulation();
		void StopSimulation();
		void ResetSimulation();
		void UpdateSimulation();
	}
}