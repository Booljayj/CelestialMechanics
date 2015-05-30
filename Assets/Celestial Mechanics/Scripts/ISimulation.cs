using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	public interface ISimulation {
		void StartSimulation();
		void EndSimulation();
		void ResetSimulation();
		void UpdateSimulation();
	}
}