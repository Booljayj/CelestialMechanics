using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CelestialMechanics {
	public class CelestialOrbit : MonoBehaviour, ISimulation {
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
		[SerializeField, FormerlySerializedAs("simulate")] bool _simulate = true;
		public bool simulate {
			get {return _simulate;}
			set {
				if (_simulate != value) {
					_simulate = value;
					if (_simulate) OnOrbitStart.Invoke();
					else OnOrbitEnd.Invoke();
				}
			}
		}

		[SerializeField] Vector2 _limits = new Vector2(-180,180); //[degrees]
		public Vector2 limits {
			get {return _limits;}
			set {
				_limits = value;
				rate = Kepler.ComputeRate(_period, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
			}
		}

		public WrapMode ending = WrapMode.Loop;

		public double meanAnomaly = 0.0; //[degrees]

		//time fields
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

		//events
		public UnityEvent OnOrbitStart;
		public OrbitEvent OnOrbitUpdate;
		public UnityEvent OnOrbitEnd;
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
			if (simulate) OnOrbitStart.Invoke();
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
			throw new NotImplementedException("Cannot add delta-v to an orbit yet");
		}

		void WrapAnomaly() {
			if (anomaly < _limits.x*Deg2Rad || anomaly > _limits.y*Deg2Rad) {
				switch (ending) {
				case WrapMode.Clamp:
					simulate = false;
					ResetSimulation();
					break;
				case WrapMode.ClampForever:
					gameObject.SetActive(false);
					ResetSimulation();
					break;
				case WrapMode.PingPong:
					if (anomaly < _limits.x*Deg2Rad) {
						anomaly = 2*_limits.x*Deg2Rad - anomaly;
					} else {
						anomaly = 2*_limits.y*Deg2Rad - anomaly;
					}
					timeScale *= -1.0;
					break;
				}

				anomaly = Kepler.WrapAngle(anomaly, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
			}
		}
		#endregion

		#region Simulation
		public void StartSimulation() {simulate = true;}
		public void StopSimulation() {simulate = false;}

		public void ResetSimulation() {
			anomaly = Kepler.WrapAngle(meanAnomaly*Deg2Rad + startEpoch * rate, _limits.x*Deg2Rad, _limits.y*Deg2Rad);
		}

		public void UpdateSimulation() {
			anomaly += Time.deltaTime * rate * timeScale;
			WrapAnomaly();
			ComputeDynamicProperties(anomaly);
			transform.localPosition = position;

			OnOrbitUpdate.Invoke(trueAnomaly);
		}
		#endregion

		#region Gizmos
		void OnDrawGizmosSelected() {
			//OnValidate should always be called before this, meaning appropriate values for properties are available

			//variables required
			Vector3[] path = new Vector3[51];
			Vector3 periapsis = orientation*Kepler.ComputePosition(Kepler.ComputeRadius(semiLatusRectum, _eccentricity, 0f), 0f);
			Vector3 positionV = orientation*Kepler.ComputePosition(Kepler.ComputeRadius(semiLatusRectum, _eccentricity, trueAnomaly), trueAnomaly);

			//build list of vectors for path
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

			//Set the gizmos to draw in parent space
			Gizmos.matrix = (transform.parent)? transform.parent.localToWorldMatrix : Matrix4x4.identity;

			//draw the path of the orbit
			Gizmos.color = Color.cyan;
			for (int i = 0; i < 50; i++) {
				Gizmos.DrawLine(orientation*path[i], orientation*path[i+1]);
			}

			//draw periapsis vector
			Gizmos.DrawLine(Vector3.zero, periapsis);

			//draw position vector
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Vector3.zero, positionV);

			//draw velocity vector
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(positionV, positionV + velocity);
		}
		#endregion
	}
}