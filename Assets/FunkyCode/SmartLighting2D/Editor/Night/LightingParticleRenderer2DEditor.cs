using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


[CanEditMultipleObjects]
[CustomEditor(typeof(LightingParticleRenderer2D))]
public class LightingParticleRenderer2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingParticleRenderer2D script = target as LightingParticleRenderer2D;

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Layer (Night)", (int)script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

        script.color = EditorGUILayout.ColorField("Color", script.color);
        
        script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

        script.scale = EditorGUILayout.FloatField("Scale", script.scale);

        script.customParticle = (Texture)EditorGUILayout.ObjectField("Custom Particle", script.customParticle, typeof(Texture), true);

		if (GUI.changed){
            if (EditorApplication.isPlaying == false) {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
		}
	}
}
