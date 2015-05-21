using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

namespace CelestialMechanics {
	public class CelestialGenerator : MonoBehaviour {
		public Vector2 numberOfPlanets = new Vector2(3f, 10f);
		public Vector2 numberOfMoons = new Vector2(1f, 10f);

		public Vector2 planetSizeRange = new Vector2(.05f, .2f);
		public Vector2 moonSizeRange = new Vector2(.01f, .04f);
		public Vector2 planetAxisRange = new Vector2(1f, 100f);
		public Vector2 moonAxisRange = new Vector2(.5f, 2f);
		public float gravitationParameter = .1f;
		public Vector2 declinationRange = new Vector2(-10, 10);
		public Vector2 rotationPeriodRange = new Vector2(10, 50);

		public Vector2 eccentricityRange = new Vector2(.1f, .4f);
		public Vector2 inclinationRange = new Vector2(-10f, 10f);
		public Vector2 longitudeRange = new Vector2(-180f, 180f);
		public Vector2 argumentRange = new Vector2(-180f, 180f);

		public float planetRingChance = .1f;
		public Vector2 ringDistanceRange = new Vector2(1f, 2f);
		public Vector2 ringWidthRange = new Vector2(.1f, 1f);

		void Start() {
			int planets = Random.Range(Mathf.RoundToInt(numberOfPlanets.x), Mathf.RoundToInt(numberOfPlanets.y));
			for (int i=0; i<planets; i++) {
				GameObject planet = GenerateBody(gameObject, string.Format("planet {0}",i), planetAxisRange, planetSizeRange);

				int moons = Random.Range(Mathf.RoundToInt(numberOfMoons.x), Mathf.RoundToInt(numberOfMoons.y));
				for (int j=0; j<moons; j++) {
					GenerateBody(planet, string.Format("moon {0}",j), moonAxisRange, moonSizeRange);
				}
			}
		}

		GameObject GenerateBody(GameObject parent, string name, Vector2 axisRange, Vector2 sizeRange) {
			GameObject orbitObj = new GameObject(name);
			orbitObj.transform.parent = parent.transform;
			orbitObj.transform.localPosition = Vector3.zero;
			
			CelestialOrbit orbit = orbitObj.AddComponent<CelestialOrbit>();
			orbit.semiMajorAxis = Random.Range(axisRange.x, axisRange.y);
			orbit.period = 2*Math.PI*Math.Sqrt(orbit.semiMajorAxis*orbit.semiMajorAxis*orbit.semiMajorAxis/gravitationParameter);
			orbit.eccentricity = Random.Range(eccentricityRange.x, eccentricityRange.y);
			orbit.inclination = Random.Range(inclinationRange.x, inclinationRange.y);
			orbit.longitude = Random.Range(longitudeRange.x, longitudeRange.y);
			orbit.argument = Random.Range(argumentRange.x, argumentRange.y);
			orbit.meanAnomaly = Random.Range(-180f, 180f);
			orbit.ComputeStaticProperties();
			
			GameObject planetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			planetObj.transform.parent = orbitObj.transform;
			planetObj.transform.localScale = Random.Range(sizeRange.x, sizeRange.y) * Vector3.one;
			planetObj.transform.localPosition = Vector3.zero;
			
			CelestialRotation rotation = planetObj.AddComponent<CelestialRotation>();
			rotation.declination = Random.Range(declinationRange.x, declinationRange.y);
			rotation.rightAscension = Random.Range(-180, 180);
			rotation.period = Random.Range(rotationPeriodRange.x, rotationPeriodRange.y);
			rotation.meanAngle = Random.Range(-180, 180);
			rotation.ComputeStaticProperties();

//			if (Random.value < planetRingChance) {
//			}

			return orbitObj;
		}
	}
}