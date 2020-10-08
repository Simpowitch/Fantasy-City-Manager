using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingManager2D))]
public class LightingManager2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingManager2D script = target as LightingManager2D;

		int count = script.cameraSettings.Length;
		count = EditorGUILayout.IntField("Camera Count", count);
		if (count != script.cameraSettings.Length) {
			System.Array.Resize(ref script.cameraSettings, count);
		}

		for(int id = 0; id < script.cameraSettings.Length; id++) {
			CameraSettings cameraSetting = script.cameraSettings[id];

			EditorGUILayout.Foldout(true, "Camera (Id: " + (id + 1) + ")" );

			EditorGUI.indentLevel++;

			cameraSetting.cameraType = (CameraSettings.CameraType)EditorGUILayout.EnumPopup("Camera Type", cameraSetting.cameraType);

			if (cameraSetting.cameraType == CameraSettings.CameraType.Custom) {
				cameraSetting.customCamera = (Camera)EditorGUILayout.ObjectField(cameraSetting.customCamera, typeof(Camera), true);
			}

			cameraSetting.bufferID = EditorGUILayout.Popup("Buffer Preset", (int)cameraSetting.bufferID, Lighting2D.Profile.bufferPresets.GetBufferLayers());

			cameraSetting.renderMode = (CameraSettings.RenderMode)EditorGUILayout.EnumPopup("Render Mode", cameraSetting.renderMode);

			cameraSetting.id = id;

			script.cameraSettings[id] = cameraSetting;

			EditorGUI.indentLevel--;
		}

		EditorGUILayout.LabelField("version " + Lighting2D.VERSION_STRING);

		string buttonName = "Re-Initialize";
		if (script.version < Lighting2D.VERSION) {
			buttonName += " (Outdated)";
			GUI.backgroundColor = Color.red;
		}
		
		if (GUILayout.Button(buttonName)) {
			foreach(Transform transform in script.transform) {
				DestroyImmediate(transform.gameObject);
			}
			
			script.Initialize();

			foreach(LightingSource2D buffer in LightingSource2D.GetList()) {
				buffer.ForceUpdate();
			}
		
			LightingManager2D.Get();
			
			LightingManager2D.ForceUpdate();
		}

		if (GUI.changed) {
			LightingManager2D.ForceUpdate();

			if (EditorApplication.isPlaying == false) {
				
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				
			}
		}
	}
}
