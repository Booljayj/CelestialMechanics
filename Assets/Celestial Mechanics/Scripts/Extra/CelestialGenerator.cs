using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

namespace CelestialMechanics {
	public class CelestialGenerator : MonoBehaviour {
		public float gravitationParameter = 10f;

		[Header("Orbital Planes")]
		public Vector2 inclinationRange = new Vector2(-10f, 10f);
		public Vector2 longitudeRange = new Vector2(-180f, 180f);
		public Vector2 argumentRange = new Vector2(-180f, 180f);

		[Header("Planets")]
		public Color planetColor;
		public Vector2 numberOfPlanets = new Vector2(50f, 100f);
		public Vector2 planetEccentricityRange = new Vector2(.1f, .4f);
		public Vector2 planetSizeRange = new Vector2(.05f, .2f);
		public Vector2 planetDistanceRange = new Vector2(1f, 100f);

		[Header("Moons")]
		public Color moonColor;
		public Vector2 numberOfMoons = new Vector2(1f, 10f);
		public Vector2 moonSizeRange = new Vector2(.01f, .04f);
		public Vector2 moonDistanceRange = new Vector2(.5f, 2f);

		[Header("Comets")]
		public Color cometColor;
		public Vector2 numberOfComets = new Vector2(5f, 10f);
		public Vector2 cometEccentricityRange = new Vector2(.8f, .9f);
		public Vector2 cometSizeRange = new Vector2(.01f, .04f);
		public Vector2 cometDistanceRange = new Vector2(10f, 20f);

		void Start() {
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			//Generate Planets
			int planets = Random.Range(Mathf.RoundToInt(numberOfPlanets.x), Mathf.RoundToInt(numberOfPlanets.y));
			int moonsTotal = 0;
			for (int i=0; i<planets; i++) {
				GameObject planet = GenerateBody(gameObject, 
				                                 string.Format("planet {0}",i),
				                                 planetColor, 
				                                 planetEccentricityRange, 
				                                 planetDistanceRange, 
				                                 planetSizeRange);

				//Generate Moons
				int moons = Random.Range(Mathf.RoundToInt(numberOfMoons.x), Mathf.RoundToInt(numberOfMoons.y));
				for (int j=0; j<moons; j++) {
					GenerateBody(planet, 
					             string.Format("moon {0}",j), 
					             moonColor,
					             Vector2.zero, //moons have a circular orbit
					             moonDistanceRange, 
					             moonSizeRange);
				}
				moonsTotal += moons;
			}

			//Generate Comets
			int comets = Random.Range(Mathf.RoundToInt(numberOfComets.x), Mathf.RoundToInt(numberOfComets.y));
			for (int k=0; k<comets; k++) {
				GenerateBody(gameObject,
				             string.Format("comet {0}",k),
				             cometColor,
				             cometEccentricityRange,
				             cometDistanceRange,
				             cometSizeRange);
			}

			watch.Stop();
			Debug.Log(string.Format("Generated {0} planets, {1} moons, and {2} comets in {3}ms",
			          planets, moonsTotal, comets, watch.ElapsedMilliseconds));
		}

		GameObject GenerateBody(GameObject parent, string name, Color color, Vector2 eccentricityRange, Vector2 distanceRange, Vector2 sizeRange) {
			GameObject orbitObj = new GameObject(name);
			orbitObj.transform.parent = parent.transform;
			orbitObj.transform.localPosition = Vector3.zero;

			//add an orbit component, and fill it out
			CelestialOrbit orbit = orbitObj.AddComponent<CelestialOrbit>();
			orbit.periapsis = Random.Range(distanceRange.x, distanceRange.y);
			orbit.eccentricity = Random.Range(eccentricityRange.x, eccentricityRange.y);
			orbit.inclination = Random.Range(inclinationRange.x, inclinationRange.y);
			orbit.longitude = Random.Range(longitudeRange.x, longitudeRange.y);
			orbit.argument = Random.Range(argumentRange.x, argumentRange.y);
			orbit.meanAnomaly = Random.Range(-180f, 180f);
			orbit.ComputeStaticProperties(); //need to do this so that semi major axis is available
			orbit.period = 2*Math.PI*Math.Sqrt(orbit.semiMajorAxis*orbit.semiMajorAxis*orbit.semiMajorAxis/gravitationParameter);

			//create the model for the body
			GameObject planetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			planetObj.GetComponent<Renderer>().material.color = color;
			planetObj.transform.parent = orbitObj.transform;
			planetObj.transform.localScale = Random.Range(sizeRange.x, sizeRange.y) * Vector3.one;
			planetObj.transform.localPosition = Vector3.zero;

			return orbitObj;
		}
	}
}