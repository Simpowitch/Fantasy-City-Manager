using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingCollider2D))]
public class LightingCollider2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingCollider2D script = target as LightingCollider2D;

		script.mainShape.colliderType = (LightingCollider2D.ColliderType)EditorGUILayout.EnumPopup("Shadow Type", script.mainShape.colliderType);
		

		EditorGUI.BeginDisabledGroup(script.mainShape.colliderType == LightingCollider2D.ColliderType.None);
		
		script.lightingCollisionLayer = (LightingLayer)EditorGUILayout.Popup("Shadow Layer (Light)", (int)script.lightingCollisionLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

		string shadowDistanceName = "Shadow Distance";

		if (script.mainShape.shadowDistance == 0) {
			shadowDistanceName = "Shadow Distance (infinite)";
		}

		script.shadowEffectLayer = (LightingLayer)EditorGUILayout.Popup("Shadow Effect Layer (Light)", (int)script.shadowEffectLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

		script.mainShape.shadowDistance = EditorGUILayout.FloatField(shadowDistanceName, script.mainShape.shadowDistance);

		if (script.mainShape.shadowDistance < 0) {
			script.mainShape.shadowDistance = 0;
		}

	
		EditorGUI.EndDisabledGroup();
		

		EditorGUILayout.Space();

		script.mainShape.maskType = (LightingCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.mainShape.maskType);


		EditorGUI.BeginDisabledGroup(script.mainShape.maskType == LightingCollider2D.MaskType.None);

		script.lightingMaskLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Light)", (int)script.lightingMaskLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

		script.maskEffect = (MaskEffect)EditorGUILayout.EnumPopup("Mask Effect", script.maskEffect);

		EditorGUI.EndDisabledGroup();
		
		if (script.mainShape.maskType == LightingCollider2D.MaskType.BumpedSprite) {
			GUIBumpMapMode.Draw(script.normalMapMode);
		}

		EditorGUILayout.Space();

		script.applyToChildren = EditorGUILayout.Toggle("Apply To Children", script.applyToChildren);

		EditorGUILayout.Space();

		Update();

		ApplyToAll();

		if (GUI.changed) {
			script.Initialize();
			script.UpdateNearbyLights();
			LightingManager2D.ForceUpdate();

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}

	void Update() {
		LightingCollider2D script = target as LightingCollider2D;

		if (GUILayout.Button("Update")) {
			CustomPhysicsShapeManager.Clear();

			script.Initialize();

			LightingManager2D.ForceUpdate();
		}
	}

	void ApplyToAll() {
		LightingCollider2D script = target as LightingCollider2D;

		if (targets.Length > 1) {
			if (GUILayout.Button("Apply to All")) {
				foreach(Object obj in targets) {
					LightingCollider2D copy = obj as LightingCollider2D;
					if (copy == script) {
						continue;
					}

					copy.mainShape.colliderType = script.mainShape.colliderType;
					copy.lightingCollisionLayer = script.lightingCollisionLayer;

					copy.mainShape.maskType = script.mainShape.maskType;
					copy.lightingMaskLayer = script.lightingMaskLayer;

					copy.Initialize();
				}

				LightingManager2D.ForceUpdate();
			}
		}
	}
}
