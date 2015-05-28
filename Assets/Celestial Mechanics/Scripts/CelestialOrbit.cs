using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CelestialMechanics {
	public class CelestialOrbit : MonoBehaviour {
		const double Deg2Rad = Math.PI/180.0;

		#region Fields
		//input fields
		[SerializeField] double _semiMajorAxis = 1.0; //[m]
		public double semiMajorAxis {
			get {return _semiMajorAxis;}
			set {
				_semiMajorAxis = value;
				semiLatusRectum = Kepler.ComputeSemiLatusRectum(_semiMajorAxis, _eccentricity);
			}
		}

		[SerializeField] double _eccentricity = 0.4; //[1]
		public double eccentricity {
			get {return _eccentricity;}
			set {
				_eccentricity = value;
				semiLatusRectum = Kepler.ComputeSemiLatusRectum(_semiMajorAxis, _eccentricity);
			}
		}

		[SerializeField] float _argument = 0.0f; //[degrees]
		public float argument {
			get {return _argument;}
			set {
				_argument = value;
				orientation = Kepler.ComputeOrientation(_argument, _longitude, _inclination);
			}
		}

		[SerializeField] float _longitude = 0.0f; //[degrees]
		public float longitude {
			get {return _longitude;}
			set {
				_longitude = value;
				orientation = Kepler.ComputeOrientation(_argument, _longitude, _inclination);
			}
		}

		[SerializeField] float _inclination = 0.0f; //[degrees]
		public float inclination {
			get {return _inclination;}
			set {
				_inclination = value;
				orientation = Kepler.ComputeOrientation(_argument, _longitude, _inclination);
			}
		}

		//control fields
		public bool simulate = true;

		[SerializeField] Vector2 _limits = new Vector2(-180,180); //[degrees]
		public Vector2 limits {
			get {return _limits;}
			set {
				_limits = value;
				rate = Kepler.ComputeRate(_period, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
			}
		}

		public double meanAnomaly = 0.0; //[degrees]

		[SerializeField] double _period = 10; //[seconds/orbit]
		public double period {
			get {return _period;}
			set {
				_period = value;
				rate = Kepler.ComputeRate(_period, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
			}
		}

		public double timeScale = 1.0;

		public double startEpoch = 0.0; //[seconds]
		#endregion

		#region Properties
		//static properties
		public Quaternion orientation {get; private set;}
		public double semiLatusRectum {get; private set;} //[m]
		public double rate {get; private set;} //[rad/second]

		//dynamic properties
		public double anomaly {get; private set;} //[rad]
		public double eccentricAnomaly {get; private set;} //[rad]
		public double trueAnomaly {get; private set;} //[rad]
		public double radius {get; private set;} //[m]
		public Vector3 position {get; private set;}
		public Vector3 velocity {get; private set;}
		#endregion

		#region Messages
		void Start() {
			ResetSimulation();
		}

		void OnEnable() {
			ComputeStaticProperties();
			ComputeDynamicProperties(anomaly);
			transform.localRotation = orientation;
			transform.localPosition = position;
		}

		void Update() {
			if (simulate) {UpdateSimulation();}
		}

		void OnValidate() {
			if (_eccentricity < 0) _eccentricity *= -1.0;
			if (_semiMajorAxis < 0) _semiMajorAxis *= -1.0;

			Start();
			OnEnable();
			SetupGizmos();
		}
		#endregion

		#region Computation
		/// <summary>Compute static properties for orbit shape and speed</summary>
		public void ComputeStaticProperties() {
			orientation = Kepler.ComputeOrientation(argument, longitude, inclination);
			semiLatusRectum = Kepler.ComputeSemiLatusRectum(semiMajorAxis, eccentricity);
			rate = Kepler.ComputeRate(period, limits.x*Deg2Rad, limits.y*Deg2Rad);
		}

		/// <summary>Compute dynamic properties that change over the anomaly</summary>
		/// <param name="M">The anomaly to evaluate properties at</param>
		public void ComputeDynamicProperties(double M) {
			eccentricAnomaly = Kepler.ComputeEccentricAnomaly(M, eccentricity);
			trueAnomaly = Kepler.ComputeTrueAnomaly(eccentricAnomaly, eccentricity);
			radius = Kepler.ComputeRadius(semiLatusRectum, eccentricity, trueAnomaly);
			position = orientation * Kepler.ComputePosition(radius, trueAnomaly);
			velocity = orientation * Kepler.ComputeVelocity(_semiMajorAxis, radius, rate, eccentricAnomaly, eccentricity);
		}

		//TODO: Define function for adding dV
		//requires dynamic properties to be set up in ComputeProperties for initial conditions
		//not strictly necessary, but would greatly help when integrating physics
		public void AddVelocity(Vector3 dV) {
			
		}
		#endregion

		#region Simulation
		public void StartSimulation() {simulate = true;}
		public void StopSimulation() {simulate = false;}

		public void ResetSimulation() {
			anomaly = Kepler.WrapAngle(meanAnomaly*Deg2Rad + startEpoch * rate, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
		}

		public void UpdateSimulation() {
			anomaly = Kepler.WrapAngle(anomaly + Time.deltaTime * rate * timeScale, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
			ComputeDynamicProperties(anomaly);
			transform.localPosition = position;
		}
		#endregion

		#region Gizmos
		Vector3[] path;

		void SetupGizmos() {
			path = new Vector3[51];
			double step, lower;
			double E, v, r;
			
			step = (_limits.y - _limits.x)*Deg2Rad/50;
			lower = _limits.x*Deg2Rad;
			
			for (int i = 0; i <= 50; i++) {
				E = Kepler.ComputeEccentricAnomaly(lower+step*i, _eccentricity);
				v = Kepler.ComputeTrueAnomaly(E, _eccentricity);
				r = Kepler.ComputeRadius(semiLatusRectum, _eccentricity, v);
				path[i] = Kepler.ComputePosition(r, v);
			}
		}

		void OnDrawGizmosSelected() {
			if (path == null || path.GetLength(0) < 2) return; //sanity check, should never trip

			Gizmos.matrix = (transform.parent)? transform.parent.localToWorldMatrix : Matrix4x4.identity;

			Gizmos.color = Color.cyan;
			for (int i = 0; i < 50; i++) {
				//Debug.Log(orientation);
				//Debug.Log(path[i]);
				Gizmos.DrawLine(orientation*path[i], orientation*path[i+1]);
			}
			Gizmos.DrawLine(Vector3.zero, orientation*Kepler.ComputePosition(Kepler.ComputeRadius(semiLatusRectum, eccentricity, 0f), 0f));

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Vector3.zero, orientation*Kepler.ComputePosition(Kepler.ComputeRadius(semiLatusRectum, eccentricity, trueAnomaly), trueAnomaly));
		}
		#endregion
	}
}