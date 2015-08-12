using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;

namespace CelestialMechanics {
	[CustomEditor(typeof(CelestialOrbit))]
	public class CelestialOrbitEditor : Editor {
		SerializedProperty a, e, argument, longitude, inclination;
		SerializedProperty sim, limits, M0, T, tscale, epoch;
		SerializedProperty start, update, end;
		AnimBool closedOrbit, openOrbit;

		//conic section
		GUIContent eGUI = new GUIContent("Eccentricity",
			"Range: 0 to Inf \n" +
		    "Deviation of the conic section from a circle");
		//a-value
		GUIContent aGUI_circle = new GUIContent("Radius",
			"Range: 0 to Inf \n" +
			"The distance from the focus of the orbit");
		GUIContent aGUI_ellipse = new GUIContent("Periapsis",
			"Range: 0 to Inf \n" +
			"The distance to the closest point of the ellipse");
		GUIContent aGUI_open = new GUIContent("Focal Length",
			"Range: 0 to Inf \n" +
			"The distance between the focus and the vertex");
		//orientation
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
		//simulation
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
		//orbit-specific parameters
		GUIContent epoGUI = new GUIContent("Epoch",
			"Range: 0 to Inf \n" +
			"Starting time of the simulation");
		GUIContent limGUI = new GUIContent("Limits",
			"Range: -Inf to Inf \n" +
			"Lower and Upper anomaly limits");

		void OnEnable() {
			e = serializedObject.FindProperty("_eccentricity");
			a = serializedObject.FindProperty("_periapsis");
			argument = serializedObject.FindProperty("_argument");
			longitude = serializedObject.FindProperty("_longitude");
			inclination = serializedObject.FindProperty("_inclination");

			sim = serializedObject.FindProperty("_simulate");
			M0 = serializedObject.FindProperty("meanAnomaly");
			T = serializedObject.FindProperty("_period");
			tscale = serializedObject.FindProperty("timeScale");

			epoch = serializedObject.FindProperty("startEpoch");
			limits = serializedObject.FindProperty("_limits");

			start = serializedObject.FindProperty("OnOrbitStart");
			update = serializedObject.FindProperty("OnOrbitUpdate");
			end = serializedObject.FindProperty("OnOrbitEnd");
			
			closedOrbit = new AnimBool(e.doubleValue >= 1? false : true, Repaint);
			openOrbit = new AnimBool(e.doubleValue < 1? false : true, Repaint);
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			//show eccentricity field, and check if eccentricity defines an open orbit
			EditorGUILayout.PropertyField(e, eGUI);
			if (e.doubleValue < 1) {
				closedOrbit.target = true;
				openOrbit.target = false;
			} else {
				closedOrbit.target = false;
				openOrbit.target = true;
			}

			//show the appropriate a-value GUI for the chosen eccentricity
			if (e.doubleValue == 0.0) {			//circular
				EditorGUILayout.PropertyField(a, aGUI_circle);
			} else if (e.doubleValue < 1.0) {	//elliptical
				EditorGUILayout.PropertyField(a, aGUI_ellipse);
			} else {							//hyperbolic && parabolic
				EditorGUILayout.PropertyField(a, aGUI_open);
			}

			//show orientation sliders
			EditorGUILayout.Slider(longitude, -180f, 180f, lonGUI);
			EditorGUILayout.Slider(inclination, -180f, 180f, incGUI);
			EditorGUILayout.Slider(argument, -180f, 180f, argGUI);
			EditorGUILayout.Separator();

			//begin controls block
			EditorGUILayout.LabelField("Control", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(sim, simGUI);

			//show time and starting position controls
			EditorGUILayout.Slider(M0, limits.vector2Value.x, limits.vector2Value.y, M0GUI);
			EditorGUILayout.PropertyField(T, TGUI);
			//only show epoch for closed orbits
			if (EditorGUILayout.BeginFadeGroup(closedOrbit.faded)) {
				EditorGUILayout.PropertyField(epoch, epoGUI);
			}
			EditorGUILayout.EndFadeGroup();
			// only show limits for open orbits
			if (EditorGUILayout.BeginFadeGroup(openOrbit.faded)) {
				EditorGUILayout.PropertyField(limits, limGUI);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(tscale, tscGUI);

			EditorGUILayout.PropertyField(start);
			EditorGUILayout.PropertyField(update);
			EditorGUILayout.PropertyField(end);

			serializedObject.ApplyModifiedProperties();
		}
	}
}