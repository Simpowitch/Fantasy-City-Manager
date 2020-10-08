using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#if UNITY_2017_4_OR_NEWER

[CustomEditor(typeof(LightingTilemapCollider2D))]
public class LightingTilemapCollider2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingTilemapCollider2D script = target as LightingTilemapCollider2D;

		script.mapType = (LightingTilemapCollider2D.MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.mapType);

		switch(script.mapType) {
			case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
				script.colliderType = (LightingTilemapCollider2D.ColliderType)EditorGUILayout.EnumPopup("Shadow Type", script.colliderType);
				script.lightingCollisionLayer = (LightingLayer)EditorGUILayout.Popup("Shadow Layer (Light)", (int)script.lightingCollisionLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());
				script.colliderTileType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Shadow Tile Type", script.colliderTileType);

				script.maskType = (LightingTilemapCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.maskType);
				script.lightingMaskLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Light)", (int)script.lightingMaskLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());

			break;

			case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
				script.superTilemapEditor.colliderType = (SuperTilemapEditorSupport.TilemapCollider2D.ColliderType)EditorGUILayout.EnumPopup("Collision Type", script.superTilemapEditor.colliderType);
				script.lightingCollisionLayer = (LightingLayer)EditorGUILayout.Popup("Collision Layer (Light)", (int)script.lightingCollisionLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());

				script.superTilemapEditor.maskType = (SuperTilemapEditorSupport.TilemapCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.superTilemapEditor.maskType);
				script.lightingMaskLayer = (LightingLayer)EditorGUILayout.Popup("Mask Layer (Light)", (int)script.lightingMaskLayer, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());
			break;
		}

		if (script.maskType == LightingTilemapCollider2D.MaskType.BumpedSprite) {
			GUIBumpMapMode.Draw(script.bumpMapMode);
		}

		UpdateCollisions(script);
		
		if (GUI.changed) {
			script.Initialize();

			LightingSource2D.ForceUpdateAll();
			LightingManager2D.ForceUpdate();

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}

	static void UpdateCollisions(LightingTilemapCollider2D script) {
		if (GUILayout.Button("Update Collisions")) {
			CustomPhysicsShapeManager.Clear();
			
			script.Initialize();

			LightingSource2D.ForceUpdateAll();
			LightingManager2D.ForceUpdate();
		}
	}
}

#endif