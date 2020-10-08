using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingBuffer2D))]
public class LightingBuffer2DEditor : Editor {
	override public void OnInspectorGUI() {
		LightingBuffer2D script = target as LightingBuffer2D;
		
		EditorGUILayout.ObjectField("Lighting Source", script.lightSource, typeof(LightingSource2D), true);

		EditorGUILayout.Toggle("Is Free", script.Free);

		EditorGUILayout.ObjectField("Render Texture", script.renderTexture, typeof(Texture), true);
	}
}