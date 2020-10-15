using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightBuffers))]
public class LightBuffersEditor : Editor {
	override public void OnInspectorGUI() {
		LightBuffers script = target as LightBuffers;

		foreach(LightingBuffer2D buffer in LightingBuffer2D.list) {
			EditorGUILayout.LabelField(buffer.name);
			EditorGUILayout.ObjectField("Lighting Source", buffer.lightSource, typeof(LightingSource2D), true);

			EditorGUILayout.Toggle("Is Free", buffer.Free);

			EditorGUILayout.ObjectField("Render Texture", buffer.renderTexture.renderTexture, typeof(Texture), true);
		}
		
		
	}
}