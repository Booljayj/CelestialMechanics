using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CelestialMechanics {

[CustomEditor(typeof(CelestialOrbit))]
public class CelestialOrbitEditor : Editor {
	SerializedProperty a, e, argument, longitude, inclination;
	SerializedProperty sim, limits, M0, T, tscale, epoch;

	GUIContent aGUI = new GUIContent("Semi-Major Axis",
		"Range: 0 to Inf \n" +
		"The longest radius of the ellipse");
	GUIContent eGUI = new GUIContent("Eccentricity",
		"Range: 0 to Inf \n" +
		"Deviation of the conic section from a circle");
	GUIContent argGUI = new GUIContent("Argument",
		"Range: -180 to 180 \n" +
		"Argument of the Periapsis \n" +
		"Rotation of ellipse within orbital plane");
	GUIContent lonGUI = new GUIContent("Longitude",
		"Range: -180 to 180 \n" +
		"Longitude of the Ascending Node \n" +
		"Rotation of the x-axis within the coordinate system");
	GUIContent incGUI = new GUIContent("Inclination",
		"Range: -180 to 180 \n" +
		"Inclination \n" +
		"Rotation about the x-axis");
	GUIContent simGUI = new GUIContent("Run Simulation",
		"Run the simulation");
	GUIContent M0GUI = new GUIContent("Mean Anomaly",
		"Range: -180 to 180 \n" +
		"Starting angle of the body along its orbit");
	GUIContent TGUI = new GUIContent("Period",
		"Range: 0 to Inf \n" +
		"Time it takes for the body to complete one orbit");
	GUIContent tscGUI = new GUIContent("Time Scale",
		"Range: 0 to Inf \n" +
		"Time scale of the simulation");
	GUIContent epoGUI = new GUIContent("Epoch",
		"Range: 0 to Inf \n" +
		"Starting time of the simulation");
	GUIContent limGUI = new GUIContent("Limits",
		"Range: -Inf to Inf \n" +
		"Lower and Upper anomaly limits");

	void OnEnable() {
		a = serializedObject.FindProperty("_semiMajorAxis");
		e = serializedObject.FindProperty("_eccentricity");
		argument = serializedObject.FindProperty("_argument");
		longitude = serializedObject.FindProperty("_longitude");
		inclination = serializedObject.FindProperty("_inclination");

		sim = serializedObject.FindProperty("simulate");
		M0 = serializedObject.FindProperty("meanAnomaly");
		T = serializedObject.FindProperty("_period");
		tscale = serializedObject.FindProperty("timeScale");
		epoch = serializedObject.FindProperty("startEpoch");

		limits = serializedObject.FindProperty("_limits");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(a, aGUI);
		EditorGUILayout.PropertyField(e, eGUI);
		EditorGUILayout.Slider(longitude, -180f, 180f, lonGUI);
		EditorGUILayout.Slider(inclination, -180f, 180f, incGUI);
		EditorGUILayout.Slider(argument, -180f, 180f, argGUI);
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Control", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(sim, simGUI);
		EditorGUILayout.PropertyField(limits, limGUI);
		EditorGUILayout.Slider(M0, limits.vector2Value.x, limits.vector2Value.y, M0GUI);
		EditorGUILayout.PropertyField(T, TGUI);
		EditorGUILayout.PropertyField(tscale, tscGUI);
		EditorGUILayout.PropertyField(epoch, epoGUI);

		serializedObject.ApplyModifiedProperties();
	}
}

}