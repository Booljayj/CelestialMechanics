using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;

namespace CelestialMechanics {
	[CustomEditor(typeof(CelestialOrbit))]
	public class CelestialOrbitEditor : Editor {
		SerializedProperty a, e, argument, longitude, inclination;
		SerializedProperty sim, limits, ending, M0, T, tscale, epoch;
		SerializedProperty start, update, end;
		AnimBool openOrbit;

		//conic section
		GUIContent eGUI = new GUIContent("Eccentricity",
		                                 "Range: 0 to Inf \n" +
		                                 "Deviation of the conic section from a circle");
		//a-value
		GUIContent aGUI_closed = new GUIContent("Semi-Major Axis",
		                                 "Range: 0 to Inf \n" +
		                                 "The longest radius of the ellipse or circle");
		GUIContent aGUI_parabolic = new GUIContent("Focal Length",
		                                 "Range: 0 to Inf \n" +
		                                 "The distance between the focus and the vertex");
		GUIContent aGUI_hyperbolic = new GUIContent("Semi-Major Axis",
		                                            "Range: 0 to Inf \n" +
		                                            "The adjusted distance between the focus and vertex");
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
		GUIContent epoGUI = new GUIContent("Epoch",
		                                   "Range: 0 to Inf \n" +
		                                   "Starting time of the simulation");
		//orbital limits and end behavior
		GUIContent limGUI_low = new GUIContent("Low",
		                                       "Range: -Inf to Inf \n" +
		                                       "Lower anomaly limit");
		GUIContent limGUI_high = new GUIContent("High",
		                                        "Range: -Inf to Inf \n" +
		                                        "Upper anomaly limit");
		GUIContent endGUI = new GUIContent("Ending",
		                                   "Behaviour when orbit reaches its limits");


		void OnEnable() {
			e = serializedObject.FindProperty("_eccentricity");
			a = serializedObject.FindProperty("_semiMajorAxis");
			argument = serializedObject.FindProperty("_argument");
			longitude = serializedObject.FindProperty("_longitude");
			inclination = serializedObject.FindProperty("_inclination");

			sim = serializedObject.FindProperty("_simulate");
			M0 = serializedObject.FindProperty("meanAnomaly");
			T = serializedObject.FindProperty("_period");
			tscale = serializedObject.FindProperty("timeScale");
			epoch = serializedObject.FindProperty("startEpoch");

			limits = serializedObject.FindProperty("_limits");

			ending = serializedObject.FindProperty("ending");

			start = serializedObject.FindProperty("OnOrbitStart");
			update = serializedObject.FindProperty("OnOrbitUpdate");
			end = serializedObject.FindProperty("OnOrbitEnd");

			openOrbit = new AnimBool(e.doubleValue < 1? false : true, Repaint);
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			//show eccentricity field, and check if eccentricity defines an open orbit
			EditorGUILayout.PropertyField(e, eGUI);
			openOrbit.target = e.doubleValue < 1? false : true;

			//show the appropriate a-value GUI for the chosen eccentricity
			if (e.doubleValue == 1.0) {
				EditorGUILayout.PropertyField(a, aGUI_parabolic);
			} else if (e.doubleValue > 1.0) {
				EditorGUILayout.PropertyField(a, aGUI_hyperbolic);
			} else {
				EditorGUILayout.PropertyField(a, aGUI_closed);
			}

			//show orientation sliders
			EditorGUILayout.Slider(longitude, -180f, 180f, lonGUI);
			EditorGUILayout.Slider(inclination, -180f, 180f, incGUI);
			EditorGUILayout.Slider(argument, -180f, 180f, argGUI);
			EditorGUILayout.Separator();

			//begin controls block
			EditorGUILayout.LabelField("Control", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(sim, simGUI);

			//only show limits and ending behaviour for open orbits
			if (EditorGUILayout.BeginFadeGroup(openOrbit.faded)) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(limGUI_low, GUILayout.Width(30f));
				EditorGUILayout.FloatField(limits.vector2Value.x);
				EditorGUILayout.LabelField(limGUI_high, GUILayout.Width(30f));
				EditorGUILayout.FloatField(limits.vector2Value.y);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(ending, endGUI);
			}
			EditorGUILayout.EndFadeGroup();

			//if orbits are closed, their limits and ending behavior are pre-defined, only update if gui has changed
			if (!openOrbit.value && GUI.changed) {
				limits.vector2Value = new Vector2(-180f, 180f);
				ending.enumValueIndex = 0;
			}

			//show time and starting position controls
			EditorGUILayout.Slider(M0, limits.vector2Value.x, limits.vector2Value.y, M0GUI);
			EditorGUILayout.PropertyField(T, TGUI);
			EditorGUILayout.PropertyField(tscale, tscGUI);
			EditorGUILayout.PropertyField(epoch, epoGUI);

			EditorGUILayout.PropertyField(start);
			EditorGUILayout.PropertyField(update);
			EditorGUILayout.PropertyField(end);

			serializedObject.ApplyModifiedProperties();
		}
	}
}