using UnityEngine;
using System.Collections;
using System;

namespace CelestialMechanics {

public class CelestialRotation : MonoBehaviour {
	const double Deg2Rad = Math.PI/180.0;
	const double Tau = Math.PI*2.0;

	//input fields
	public float rightAscension = 0f;
	public float declination = 0f;

	//control fields
	public bool simulate = true;
	public float meanAngle = 0.0f;
	public double period = 10.0;
	public double timeScale = 1.0;
	public double startEpoch = 0.0;

	//static properties
	public Quaternion axis {get; private set;}
	public double rate {get; private set;}

	//dynamic properties
	public double angle {get; private set;}
	public Quaternion rotation {get; private set;}
	public Vector3 angularVelocity {get; private set;}

	void Start() {
		ResetSimulation();
	}

	void OnEnable() {
		ComputeStaticProperties();
		ComputeDynamicProperties(meanAngle*Deg2Rad + startEpoch * rate);
		transform.localRotation = rotation;
	}

	void Update() {
		if (simulate) UpdateSimulation();
	}

	public void ComputeStaticProperties() {
		axis = Kepler.ComputeAxis(rightAscension, declination);
		rate = Kepler.ComputeRate(period, -Math.PI, Math.PI);
	}

	public void ComputeDynamicProperties(double angle) {
		rotation = Kepler.ComputeRotation(axis, angle/Deg2Rad);
		angularVelocity = Kepler.ComputeAngularVelocity(axis, rate);
	}

	void OnValidate() {
		OnEnable();
	}

	public void StartSimulation() {simulate = true;}
	public void EndSimulation() {simulate = false;}

	public void ResetSimulation() {
		angle = Kepler.WrapAngle(meanAngle*Deg2Rad + startEpoch * rate, 0, 2*Math.PI);
	}

	public void UpdateSimulation() {
		angle = Kepler.WrapAngle(angle + Time.deltaTime * rate * timeScale, 0, 2*Math.PI);
		ComputeDynamicProperties(angle);
		transform.localRotation = rotation;
	}

	void OnDrawGizmosSelected() {
		Gizmos.DrawRay(transform.position, transform.up);
		Gizmos.DrawRay(transform.position, transform.right);
	}
}

}