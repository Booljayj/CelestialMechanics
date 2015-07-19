using UnityEngine;
using System.Collections;

namespace CelestialMechanics {
	public class CelestialCometGenerator : MonoBehaviour {
		public GameObject model;

		public float rate = 10;
		public Vector2 eccentricityRange = new Vector2(1, 5);
		public Vector2 semiMajorAxisRange = new Vector2(1, 5);
		public Vector2 periodRange = new Vector2(5, 15);
		public Vector2 sizeRange = new Vector2(.1f, .5f);

		void Start() {
			StartCoroutine(Generate());
		}

		IEnumerator Generate() {
			while (true) {
				GameObject comet = Instantiate<GameObject>(model);
				CelestialOrbit orbit = comet.GetComponent<CelestialOrbit>();
				if (orbit == null) orbit = comet.AddComponent<CelestialOrbit>();

				comet.transform.parent = transform;
				comet.transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);

				orbit.eccentricity = Random.Range(eccentricityRange.x, eccentricityRange.y);
				orbit.periapsis = Random.Range(semiMajorAxisRange.x, semiMajorAxisRange.y);
				orbit.period = Random.Range(periodRange.x, periodRange.y);

				orbit.limits = new Vector2(-360, 360);
				orbit.meanAnomaly = -360f;

				orbit.longitude = Random.Range(-180, 180);
				orbit.inclination = Random.Range(-180, 180);
				orbit.argument = Random.Range(-180, 180);

				orbit.ResetSimulation();

				comet.SetActive(true);

				DestroyOnDisable destroy = comet.AddComponent<DestroyOnDisable>();
				orbit.OnOrbitEnd.AddListener(destroy.OnDisable);

				yield return new WaitForSeconds(rate);
			}
		}
	}
}