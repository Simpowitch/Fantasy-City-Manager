using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingTextureRenderer2D))]
public class LightingTextureRenderer2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingTextureRenderer2D script = target as LightingTextureRenderer2D;

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Layer (Night)", (int)script.nightLayer, Lighting2D.ProjectSettings.layers.nightLayers.GetNames());

		script.color = EditorGUILayout.ColorField("Color", script.color);

		script.size = EditorGUILayout.Vector2Field("Size", script.size);

		script.texture = (Texture)EditorGUILayout.ObjectField("Texture", script.texture, typeof(Texture), true);

		if (GUI.changed){

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}
