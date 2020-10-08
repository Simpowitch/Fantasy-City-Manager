using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingTilemapOcclusion2D))]
public class LightingTilemap2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingTilemapOcclusion2D script = target as LightingTilemapOcclusion2D;

        script.tilemapType = (LightingTilemapOcclusion2D.MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

        script.onlyColliders = EditorGUILayout.Toggle("Only Colliders", script.onlyColliders);

		GUISortingLayer.Draw(script.sortingLayer);
        
        if (GUILayout.Button("Update Collisions")) {
			script.Initialize();
		}

		if (GUI.changed) {
			script.Initialize();

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
    }
}
