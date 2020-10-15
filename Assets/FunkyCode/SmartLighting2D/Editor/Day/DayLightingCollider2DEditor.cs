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

		script.mainShape.colliderType = (DayLightingCollider2D.ColliderType)EditorGUILayout.EnumPopup("Shadow Type", script.mainShape.colliderType);


		EditorGUI.BeginDisabledGroup(script.mainShape.colliderType == DayLightingCollider2D.ColliderType.None);
		
		script.collisionDayLayer =  (LightingLayer)EditorGUILayout.Popup("Shadow Layer (Day)", (int)script.collisionDayLayer, Lighting2D.ProjectSettings.layers.dayLayers.GetNames());
		
		script.mainShape.height = EditorGUILayout.FloatField("Shadow Height", script.mainShape.height);

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		script.mainShape.maskType = (DayLightingCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.mainShape.maskType);
		
		
		EditorGUI.BeginDisabledGroup(script.mainShape.maskType == DayLightingCollider2D.MaskType.None);

		script.maskDayLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Day)", (int)script.maskDayLayer, Lighting2D.ProjectSettings.layers.dayLayers.GetNames());
		
		if (script.mainShape.maskType == DayLightingCollider2D.MaskType.BumpedSprite) {
			GUIBumpMapMode.DrawDay(script.normalMapMode);
		}

		EditorGUI.EndDisabledGroup();

	
		EditorGUI.BeginDisabledGroup(script.mainShape.colliderType == DayLightingCollider2D.ColliderType.None);

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		script.applyToChildren = EditorGUILayout.Toggle("Apply To Children", script.applyToChildren);

		EditorGUILayout.Space();

		
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

			script.mainShape.ResetLocal();

			script.Initialize();
		}
	}
}
