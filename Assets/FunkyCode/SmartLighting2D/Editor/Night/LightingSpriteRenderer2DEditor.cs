using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingSpriteRenderer2D))]
public class LightingSpriteRenderer2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingSpriteRenderer2D script = target as LightingSpriteRenderer2D;

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Layer (Night)", (int)script.nightLayer, Lighting2D.ProjectSettings.layers.nightLayers.GetNames());

        script.type = (LightingSpriteRenderer2D.Type)EditorGUILayout.EnumPopup("Type", script.type);

        script.spriteMode = (LightingSpriteRenderer2D.SpriteMode)EditorGUILayout.EnumPopup("Sprite Mode", script.spriteMode);

 
        if (script.spriteMode == LightingSpriteRenderer2D.SpriteMode.Custom) {
            bool foldout0 = GUIFoldout.Draw("Virual Sprite Renderer", script);

            if (foldout0) {
                EditorGUI.indentLevel++;

                script.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", script.sprite, typeof(Sprite), true);    

                script.color = EditorGUILayout.ColorField("Color", script.color);

                script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

                script.flipX = EditorGUILayout.Toggle("Flip X", script.flipX);
                script.flipY = EditorGUILayout.Toggle("Flip Y", script.flipY);

                EditorGUI.indentLevel--;
            }
        } else {
            script.color = EditorGUILayout.ColorField("Color", script.color);

            script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);
        }

        bool foldout = GUIFoldout.Draw("Transform Offset", script.transformOffset);

        if (foldout) {
             EditorGUI.indentLevel++;

            script.transformOffset.applyTransformRotation = EditorGUILayout.Toggle("Apply Rotation Transform", script.transformOffset.applyTransformRotation);
            script.transformOffset.offsetRotation = EditorGUILayout.FloatField("Offset Rotation", script.transformOffset.offsetRotation);

            script.transformOffset.offsetPosition = EditorGUILayout.Vector2Field("Offset Position", script.transformOffset.offsetPosition);
            script.transformOffset.offsetScale = EditorGUILayout.Vector2Field("Offset Scale", script.transformOffset.offsetScale);

            EditorGUI.indentLevel--;
        }
       
        GUIMeshMode.Draw(script.meshMode);

        GUIGlowMode.Draw(script.glowMode);
	
		if (GUI.changed){
            if (EditorApplication.isPlaying == false) {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
		}
	}
}