using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingMainBuffer2D))]
public class LightingMainBuffer2DEditor : Editor {
	override public void OnInspectorGUI() {
		LightingMainBuffer2D script = target as LightingMainBuffer2D;

		EditorGUILayout.ObjectField("Camera Target", script.cameraSettings.GetCamera(), typeof(Camera), true);
		
		EditorGUILayout.EnumPopup("Camera Type", script.cameraSettings.cameraType);
		EditorGUILayout.EnumPopup("Render Mode", script.cameraSettings.renderMode);

		EditorGUILayout.ObjectField("Render Texture", script.renderTexture, typeof(Texture), true);

	}
}