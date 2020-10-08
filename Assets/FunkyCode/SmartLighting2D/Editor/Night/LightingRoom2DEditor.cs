using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingRoom2D))]
public class LightingRoom2DEditor :Editor {
    override public void OnInspectorGUI() {
		LightingRoom2D script = target as LightingRoom2D;

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Night Layer", (int)script.nightLayer, Lighting2D.ProjectSettings.layers.nightLayers.GetNames());

		script.shape.type = (LightingRoom2D.RoomType)EditorGUILayout.EnumPopup("Room Type", script.shape.type);

 		script.color = EditorGUILayout.ColorField("Color", script.color);

		Update(); 

		if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(script);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

				LightingManager2D.ForceUpdate();
			}
		}
	}

	void Update() {
		LightingRoom2D script = target as LightingRoom2D;

		if (GUILayout.Button("Update")) {
			script.Initialize();
		}
	}
}
