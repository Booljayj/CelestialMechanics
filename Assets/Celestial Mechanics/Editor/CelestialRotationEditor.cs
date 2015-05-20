using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CelestialMechanics {

[CustomEditor(typeof(CelestialRotation))]
public class CelestialRotationEditor : Editor {
	SerializedProperty rightAscension, declination;
	SerializedProperty sim, a0, T, tscale, epoch;

	GUIContent raGUI = new GUIContent("Right Ascension",
		"Range: 0 to 360 \n" +
		"Direction of the axis");
	GUIContent decGUI = new GUIContent("Declination",
		"Range: 0 to 360 \n" +
		"Steepness of axis");

	GUIContent simGUI = new GUIContent("Simulate",
		"Run the simulation");
	GUIContent a0GUI = new GUIContent("Mean Angle",
		"Range: -180 to 180 \n" +
		"Starting angle of the body around its axis");
	GUIContent TGUI = new GUIContent("Period",
		"Range: 0 to Inf \n" +
		"Time it takes for the body to complete one rotation");
	GUIContent tscGUI = new GUIContent("Time Scale",
		"Range: 0 to Inf \n" +
		"Time scale of the simulation");
	GUIContent epoGUI = new GUIContent("Epoch",
		"Range: 0 to Inf \n" +
		"Starting time of the simulation");

	void OnEnable() {
		rightAscension = serializedObject.FindProperty("rightAscension");
		declination = serializedObject.FindProperty("declination");

		sim = serializedObject.FindProperty("simulate");
		a0 = serializedObject.FindProperty("meanAngle");
		T = serializedObject.FindProperty("period");
		tscale = serializedObject.FindProperty("timeScale");
		epoch = serializedObject.FindProperty("startEpoch");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		EditorGUILayout.LabelField("Input");
		EditorGUILayout.Slider(rightAscension, 0f, 360f, raGUI);
		EditorGUILayout.Slider(declination, 0f, 360f, decGUI);

		EditorGUILayout.LabelField("Control");
		EditorGUILayout.PropertyField(sim, simGUI);
		EditorGUILayout.Slider(a0, 0f, 360f, a0GUI);
		EditorGUILayout.PropertyField(T, TGUI);
		EditorGUILayout.PropertyField(tscale, tscGUI);
		EditorGUILayout.PropertyField(epoch, epoGUI);

		serializedObject.ApplyModifiedProperties();
	}
}

}