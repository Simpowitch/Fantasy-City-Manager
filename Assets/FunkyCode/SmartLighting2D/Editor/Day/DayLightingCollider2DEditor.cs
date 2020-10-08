using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LightingSettings;

[CanEditMultipleObjects]
[CustomEditor(typeof(DayLightingCollider2D))]
public class DayLightingCollider2DEditor : Editor {

	static public bool foldoutbumpedSprite = false;

	override public void OnInspectorGUI() {
		DayLightingCollider2D script = target as DayLightingCollider2D;

		script.shape.colliderType = (DayLightingCollider2D.ColliderType)EditorGUILayout.EnumPopup("Shadow Type", script.shape.colliderType);


		EditorGUI.BeginDisabledGroup(script.shape.colliderType == DayLightingCollider2D.ColliderType.None);
		
		script.collisionDayLayer =  (LightingLayer)EditorGUILayout.Popup("Shadow Layer (Day)", (int)script.collisionDayLayer, Lighting2D.ProjectSettings.layers.dayLayers.GetNames());
		
		EditorGUI.EndDisabledGroup();


		script.shape.maskType = (DayLightingCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.shape.maskType);
		
		
		EditorGUI.BeginDisabledGroup(script.shape.maskType == DayLightingCollider2D.MaskType.None);

		script.maskDayLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Day)", (int)script.maskDayLayer, Lighting2D.ProjectSettings.layers.dayLayers.GetNames());
		
		EditorGUI.EndDisabledGroup();

	
		EditorGUI.BeginDisabledGroup(script.shape.colliderType == DayLightingCollider2D.ColliderType.None);

		script.shape.height = EditorGUILayout.FloatField("Height", script.shape.height);

		EditorGUI.EndDisabledGroup();
	

		if (script.shape.maskType == DayLightingCollider2D.MaskType.BumpedSprite) {
			GUIBumpMapMode.DrawDay(script.normalMapMode);
		}
		
		Update();

		if (GUI.changed) {

            if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
		
	void Update() {
		DayLightingCollider2D script = target as DayLightingCollider2D;

		if (GUILayout.Button("Update")) {
			CustomPhysicsShapeManager.Clear();

			script.shape.Reset();

			script.Initialize();
		}
	}
}
