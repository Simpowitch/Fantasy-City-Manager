using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingOcclusion2D))]
public class LightingOcclusion2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingOcclusion2D script = target as LightingOcclusion2D;

        script.shape.shadowType = (LightingOcclusion2D.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.shape.shadowType);

		script.occlusionType = (LightingOcclusion2D.OcclusionType)EditorGUILayout.EnumPopup("Occlusion Type", script.occlusionType);

		script.occlusionSize = EditorGUILayout.FloatField("Size", script.occlusionSize);
		
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
