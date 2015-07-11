using UnityEngine;
using System;
using System.Collections;

namespace CelestialMechanics {
	public class CelestialRotation : MonoBehaviour, ISimulation {
		const double Deg2Rad = Math.PI/180.0;

		#region Fields
		//input fields
		[SerializeField] float _rightAscension = 0.0f; //[deg]
		public float rightAscension {
			get {return _rightAscension;}
			set {
				_rightAscension = value;
				axis = Kepler.ComputeAxis(_rightAscension, _declination);
			}
		}

		[SerializeField] float _declination = 0.0f; //[deg]
		public float declination {
			get {return _declination;}
			set {
				_declination = value;
				axis = Kepler.ComputeAxis(_rightAscension, _declination);
			}
		}

		//control fields
		public bool simulate = true;

		public float meanAngle = 0.0f; //[deg]

		//time fields
		[SerializeField] double _period = 10.0; //[s]
		public double period {
			get {return _period;}
			set {
				_period = value;
				rate = Kepler.ComputeRate(_period, -Math.PI, Math.PI);
			}
		}

		public double timeScale = 1.0;

		public double startEpoch = 0.0; //[s]
		#endregion

		#region Properties
		//static properties
		public Quaternion axis {get; private set;}
		public double rate {get; private set;} //[rad/s]

		//dynamic properties
		public double angle {get; private set;} //[rad]
		public Quaternion rotation {get; private set;}
		#endregion

		#region Messages
		void Reset() {
			_rightAscension = 0.0f;
			_declination = 0.0f;

			simulate = true;
			meanAngle = 0.0f;

			_period = 10.0;
			timeScale = 1.0;
			startEpoch = 0.0;
		}

		void Start() {
			ResetSimulation();
		}

		void OnEnable() {
			ComputeStaticProperties();
			ComputeDynamicProperties(angle);
			transform.localRotation = rotation;
		}

		void Update() {
			if (simulate) UpdateSimulation();
		}

		void OnValidate() {
			Start();
			OnEnable();
		}
		#endregion

		#region Computation
		/// <summary>Computes static properties for rotational axis and speed</summary>
		public void ComputeStaticProperties() {
			axis = Kepler.ComputeAxis(_rightAscension, _declination);
			rate = Kepler.ComputeRate(_period, -Math.PI, Math.PI);
		}

		/// <summary>Compute dynamic properties that change over the angle</summary>
		/// <param name="M">The angle to evaluate properties at</param>
		public void ComputeDynamicProperties(double angle) {
			rotation = Kepler.ComputeRotation(axis, angle/Deg2Rad);
		}
		#endregion

		#region Simulation
		public void StartSimulation() {simulate = true;}
		public void StopSimulation() {simulate = false;}

		public void ResetSimulation() {
			angle = Kepler.WrapAngle(meanAngle*Deg2Rad + startEpoch * rate, 0, 2*Math.PI);
		}

		public void UpdateSimulation() {
			angle = Kepler.WrapAngle(angle + Time.deltaTime * rate * timeScale, 0, 2*Math.PI);
			ComputeDynamicProperties(angle);
			transform.localRotation = rotation;
		}
		#endregion
	}
}