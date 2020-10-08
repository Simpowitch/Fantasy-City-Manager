using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingSettings2D))]
public class LightingSettings2DEditor : Editor {
	override public void OnInspectorGUI() {
		LightingSettings2D script = target as LightingSettings2D;

		LightingSettings.Profile profile = script.setProfile;

		profile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Profile", profile, typeof(LightingSettings.Profile), true);

		script.initializeCopy = EditorGUILayout.Toggle("Initialize Copy", script.initializeCopy);

		script.setProfile = profile;
		
		if (GUI.changed) {

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}
