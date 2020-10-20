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

	private bool foldoutSprite = false;
	private bool foldoutBumpMap = false;
	private bool foldoutEventHandling = false;

	override public void OnInspectorGUI() {
		LightingSource2D script = target as LightingSource2D;

		EditorGUI.BeginChangeCheck ();

		script.lightPresetId = EditorGUILayout.Popup("Light Preset", (int)script.lightPresetId, Lighting2D.Profile.lightPresets.GetBufferLayers());

		script.nightLayer = (LightingLayer)EditorGUILayout.Popup("Layer (Night)", (int)script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

		EditorGUILayout.Space();

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

		EditorGUILayout.Space();

		script.size = EditorGUILayout.FloatField("Size", script.size);

		if (script.size < 0.1f) {
			script.size = 0.1f;
		}
	
		script.angle = EditorGUILayout.Slider("Spot Angle", script.angle, 0, 360);

		// Only Soft Shadow
		//script.coreSize = EditorGUILayout.FloatField("Core Size", script.coreSize);

		// Only Legacy Shadow
		script.outerAngle = EditorGUILayout.Slider("Outer Angle", script.outerAngle, 0, 60);

		EditorGUILayout.Space();

		script.litMode = (LightingSource2D.LitMode)EditorGUILayout.EnumPopup("Lit Mode", script.litMode);
		
		EditorGUILayout.Space();

		EditorGUI.BeginDisabledGroup(Lighting2D.Profile.qualitySettings.fixedLightTextureSize != LightingSettings.LightingSourceTextureSize.Custom);
		
		script.textureSize = (LightingSourceTextureSize)EditorGUILayout.Popup("Buffer Size", (int)Lighting2D.Profile.qualitySettings.fixedLightTextureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);
		
		EditorGUI.EndDisabledGroup();


		EditorGUILayout.Space();

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

		EditorGUILayout.Space();

		GUIMeshMode.Draw(script.meshMode);

		EditorGUILayout.Space();

		foldoutBumpMap = EditorGUILayout.Foldout(foldoutBumpMap, "Normal Map" );
		if (foldoutBumpMap) {
			EditorGUI.indentLevel++;

			script.bumpMap.intensity = EditorGUILayout.Slider("Intensity", script.bumpMap.intensity, 0, 2);
			script.bumpMap.depth = EditorGUILayout.Slider("Depth", script.bumpMap.depth, 0.1f, 20f);

			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Space();

		foldoutEventHandling = EditorGUILayout.Foldout(foldoutEventHandling, "Event Handling");

		if (foldoutEventHandling) {
			EditorGUI.indentLevel++;

			script.eventHandling.enable = EditorGUILayout.Toggle("Enable" , script.eventHandling.enable);

			script.eventHandling.useColliders = EditorGUILayout.Toggle("Use Colliders" , script.eventHandling.useColliders);

			script.eventHandling.useTilemapColliders = EditorGUILayout.Toggle("Use Tilemap Colliders" , script.eventHandling.useTilemapColliders);
			
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Space();
		
		script.applyRotation = EditorGUILayout.Toggle("Apply Rotation", script.applyRotation);

		EditorGUILayout.Space();
	
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

					//copy.layerSetting[0].sorting = script.layerSetting[0].sorting;
					//copy.layerSetting[1].sorting = script.layerSetting[1].sorting;
					
					copy.color = script.color;
			
					copy.size = script.size;
					copy.textureSize = script.textureSize;

					//copy.mesh.enable = script.additiveMode.enable;
					//copy.mesh.alpha = script.mesh.alpha;

					//copy.applyEventHandling = script.applyEventHandling;

					copy.whenInsideCollider = script.whenInsideCollider;

					copy.lightSprite = script.lightSprite;
					copy.sprite = script.sprite;
				}
			}
		}
	}
}
