using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using LightingSettings;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightingSource2D))]
public class LightingSource2DEditor : Editor {

	private bool foldoutLayers = false;
	private bool foldoutSprite = false;
	private bool foldoutBumpMap = false;

	override public void OnInspectorGUI() {
		LightingSource2D script = target as LightingSource2D;

		EditorGUI.BeginChangeCheck ();

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Layer (Night)", (int)script.nightLayer, Lighting2D.ProjectSettings.layers.nightLayers.GetNames());

		DrawLayers(script);

		#if UNITY_2018_1_OR_NEWER
			if (Lighting2D.commonSettings.HDR) {
				script.color = EditorGUILayout.ColorField(new GUIContent("Color"), script.color, true, true, true);
			} else {
				script.color = EditorGUILayout.ColorField("Color", script.color);
			}
		#else
			script.color = EditorGUILayout.ColorField("Color", script.lightColor);
		#endif

		script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

		script.size = EditorGUILayout.FloatField("Size", script.size);
		
		script.coreSize = EditorGUILayout.FloatField("Core Size", script.coreSize);
	
		script.angle = EditorGUILayout.Slider("Angle", script.angle, 0, 360);

		script.outerAngle = EditorGUILayout.Slider("Outer Angle", script.outerAngle, 0, 60);
		
		if (Lighting2D.lightingBufferSettings.fixedLightTextureSize != LightingSettings.LightingSourceTextureSize.Custom) {
			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Popup("Buffer Size", (int)Lighting2D.lightingBufferSettings.fixedLightTextureSize, LightingSourceSettings.LightingSourceTextureSizeArray);
				EditorGUI.EndDisabledGroup();
		} else {
			script.textureSize = (LightingSourceTextureSize)EditorGUILayout.Popup("Buffer Size", (int)Lighting2D.lightingBufferSettings.fixedLightTextureSize, LightingSourceSettings.LightingSourceTextureSizeArray);
		}

		foldoutSprite = EditorGUILayout.Foldout(foldoutSprite, "Light Sprite" );

		if (foldoutSprite) {
			EditorGUI.indentLevel++;
			script.lightSprite = (LightingSource2D.LightSprite)EditorGUILayout.EnumPopup("Light Sprite", script.lightSprite);

			if (script.lightSprite == LightingSource2D.LightSprite.Custom) {
				script.spriteFlipX = EditorGUILayout.Toggle("Flip X", script.spriteFlipX);
				script.spriteFlipY = EditorGUILayout.Toggle("Flip Y", script.spriteFlipY);
				script.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", script.sprite, typeof(Sprite), true);

			} else {
				if (script.sprite != LightingSource2D.GetDefaultSprite()) {
					script.sprite = LightingSource2D.GetDefaultSprite();
				}
			}
			EditorGUI.indentLevel--;
		}

		GUIAdditiveMode.Draw(script.additiveMode);

		foldoutBumpMap = EditorGUILayout.Foldout(foldoutBumpMap, "Normal Map" );
		if (foldoutBumpMap) {
			EditorGUI.indentLevel++;

			script.bumpMap.intensity = EditorGUILayout.Slider("Intensity", script.bumpMap.intensity, 0, 2);
			script.bumpMap.depth = EditorGUILayout.Slider("Depth", script.bumpMap.depth, 0.1f, 20f);

			EditorGUI.indentLevel--;
		}

		script.applyRotation = EditorGUILayout.Toggle("Apply Rotation", script.applyRotation);
	
		script.applyEventHandling = EditorGUILayout.Toggle("Apply Event Handling" , script.applyEventHandling);

		script.whenInsideCollider = (LightingSource2D.WhenInsideCollider)EditorGUILayout.EnumPopup("When Inside Collider", script.whenInsideCollider);
		

		ApplyToAll();

		EditorGUI.EndChangeCheck();
	
		if (GUI.changed){
			script.ForceUpdate();

            if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}

	void ApplyToAll() {
		LightingSource2D script = target as LightingSource2D;
		
		if (targets.Length > 1) {
			if (GUILayout.Button("Apply to All")) {
				foreach(Object obj in targets) {
					LightingSource2D copy = obj as LightingSource2D;
					if (copy == script) {
						continue;
					}

					copy.layerSetting[0].sorting = script.layerSetting[0].sorting;
					copy.layerSetting[1].sorting = script.layerSetting[1].sorting;
					
					copy.color = script.color;
			
					copy.size = script.size;
					copy.textureSize = script.textureSize;

					copy.additiveMode.enable = script.additiveMode.enable;
					copy.additiveMode.alpha = script.additiveMode.alpha;

					copy.applyEventHandling = script.applyEventHandling;
					copy.whenInsideCollider = script.whenInsideCollider;

					copy.lightSprite = script.lightSprite;
					copy.sprite = script.sprite;
				}
			}
		}
	}

	void DrawLayers(LightingSource2D source) {
		foldoutLayers = EditorGUILayout.Foldout(foldoutLayers, "Layers (Light)" );

		if (foldoutLayers == false) {
			return;
		}
		
		EditorGUI.indentLevel++;

		int layerCount = source.layerSetting.Length;

		layerCount = EditorGUILayout.IntField("Count", layerCount);

		if (layerCount != source.layerSetting.Length) {
			System.Array.Resize(ref source.layerSetting, layerCount );
		}

		EditorGUI.indentLevel++;

		for(int i = 0; i < source.layerSetting.Length; i++) {
			if (source.layerSetting[i] == null) {
				source.layerSetting[i] = new LayerSetting();
				if (i < 8) {
					source.layerSetting[i].layerID = (LightingLayer)i;
				}
			}
			
			source.layerSetting[i].layerID = (LightingLayer)EditorGUILayout.Popup("Layer (Id: " + (i + 1) +")", (int)source.layerSetting[i].layerID, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());
			
			EditorGUI.indentLevel++;

			source.layerSetting[i].type = (LightingLayerType)EditorGUILayout.EnumPopup("Type", source.layerSetting[i].type);
			source.layerSetting[i].sorting = (LightingLayerSorting)EditorGUILayout.EnumPopup("Sorting", source.layerSetting[i].sorting);
			source.layerSetting[i].effect = (LightingLayerEffect)EditorGUILayout.EnumPopup("Effect", source.layerSetting[i].effect);
	
	
			EditorGUI.BeginDisabledGroup(source.layerSetting[i].effect != LightingLayerEffect.AboveLit);
		
			source.layerSetting[i].maskEffectDistance = EditorGUILayout.FloatField("Effect Distance", source.layerSetting[i].maskEffectDistance);
	
			EditorGUI.EndDisabledGroup();
	

			EditorGUI.indentLevel--;

		}
		EditorGUI.indentLevel--;

		EditorGUI.indentLevel--;
	}
}
