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

		script.shape.colliderType = (LightingCollider2D.ColliderType)EditorGUILayout.EnumPopup("Shadow Type", script.shape.colliderType);
		

		EditorGUI.BeginDisabledGroup(script.shape.colliderType == LightingCollider2D.ColliderType.None);
		
		script.lightingCollisionLayer = (LightingLayer)EditorGUILayout.Popup("Shadow Layer (Light)", (int)script.lightingCollisionLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());

		EditorGUI.EndDisabledGroup();



		script.shape.maskType = (LightingCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.shape.maskType);


		EditorGUI.BeginDisabledGroup(script.shape.maskType == LightingCollider2D.MaskType.None);

		script.lightingMaskLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Light)", (int)script.lightingMaskLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());

		script.maskEffect = (MaskEffect)EditorGUILayout.EnumPopup("Mask Effect", script.maskEffect);

		EditorGUI.EndDisabledGroup();
		
		if (script.shape.maskType == LightingCollider2D.MaskType.BumpedSprite) {
			GUIBumpMapMode.Draw(script.normalMapMode);
		}

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

					copy.shape.colliderType = script.shape.colliderType;
					copy.lightingCollisionLayer = script.lightingCollisionLayer;

					copy.shape.maskType = script.shape.maskType;
					copy.lightingMaskLayer = script.lightingMaskLayer;

					copy.Initialize();
				}

				LightingManager2D.ForceUpdate();
			}
		}
	}
}
