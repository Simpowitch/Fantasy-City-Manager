using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraBuffers))]
public class CameraBuffersEditor : Editor {
	override public void OnInspectorGUI() {
		CameraBuffers script = target as CameraBuffers;

		foreach(LightingMainBuffer2D buffer in LightingMainBuffer2D.list) {
			EditorGUILayout.ObjectField("Camera Target", buffer.cameraSettings.GetCamera(), typeof(Camera), true);
			
			EditorGUILayout.EnumPopup("Camera Type", buffer.cameraSettings.cameraType);
			EditorGUILayout.EnumPopup("Render Mode", buffer.cameraSettings.renderMode);
			EditorGUILayout.EnumPopup("Render Shader", buffer.cameraSettings.renderShader);
			EditorGUILayout.ObjectField("Render Texture", buffer.renderTexture.renderTexture, typeof(Texture), true);
		}
	}
}