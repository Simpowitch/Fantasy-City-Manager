using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#if UNITY_2017_4_OR_NEWER

[CustomEditor(typeof(LightingTilemapRoom2D))]
public class LightingTilemapRoom2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingTilemapRoom2D script = target as LightingTilemapRoom2D;

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Night Layer", (int)script.nightLayer, Lighting2D.ProjectSettings.layers.nightLayers.GetNames());

		script.mapType = (LightingTilemapRoom2D.MapType)EditorGUILayout.EnumPopup("Map Type", script.mapType);

		script.maskType = (LightingTilemapRoom2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.maskType);

		script.shaderType = (LightingTilemapRoom2D.ShaderType)EditorGUILayout.EnumPopup("Shader Type", script.shaderType);

		if (script.shaderType == LightingTilemapRoom2D.ShaderType.ColorMask) {
			script.color = EditorGUILayout.ColorField("Shader Color", script.color);
		}
	
		if (GUILayout.Button("Update")) {
			CustomPhysicsShapeManager.Clear();
			
			script.Initialize();
			LightingManager2D.ForceUpdate();
		}

		if (GUI.changed) {
			// script.Initialize();

			LightingManager2D.ForceUpdate();
			
			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(script);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}

#endif