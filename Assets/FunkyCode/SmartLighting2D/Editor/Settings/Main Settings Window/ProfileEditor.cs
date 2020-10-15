using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;

public class ProfileEditor {
	
	public static void Draw() {
		EditorGUI.BeginChangeCheck ();

		LightingSettings.Profile profile = Lighting2D.Profile;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.ObjectField("Current Profile", profile, typeof(LightingSettings.Profile), true);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		if (profile == null) {
			EditorGUILayout.HelpBox("Lighting2D Settings Profile Not Found!", MessageType.Error);

			return;
		}

		CommonSettings(profile.bufferPresets.list[0]);

		EditorGUILayout.Space();

		SortingLayer(profile.bufferPresets.list[0].sortingLayer);

		EditorGUILayout.Space();

		QualitySettings.Draw(profile);

		EditorGUILayout.Space();

		DayLighting.Draw(profile);
		
		EditorGUILayout.Space();
		
		FogOfWar.Draw(profile);

		EditorGUILayout.Space();

		BufferPresets.Draw(profile.bufferPresets);

		EditorGUILayout.Space();

		LightPresets.Draw(profile.lightPresets);

		EditorGUILayout.Space();

		profile.disable = EditorGUILayout.Toggle("Disable", profile.disable);

		EditorGUI.EndChangeCheck ();

		if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {
				LightingSource2D.ForceUpdateAll();
				
				LightingManager2D.ForceUpdate();

				foreach(OnRenderMode onRender in OnRenderMode.list) {
					BufferPreset bufferPreset = onRender.mainBuffer.GetBufferPreset();
					bufferPreset.sortingLayer.ApplyToMeshRenderer(onRender.meshRenderer);
				}

				EditorUtility.SetDirty(profile);
			}
		}
	}

	public class LightPresets {
		public static void Draw(LightPresetList lightPresetList) {
			bool foldout = GUIFoldoutHeader.Begin( "Light Presets (" + lightPresetList.list.Length + ")", lightPresetList);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

			int bufferCount = EditorGUILayout.IntSlider ("Count", lightPresetList.list.Length, 1, 4);

			if (bufferCount !=lightPresetList.list.Length) {
				int oldCount = lightPresetList.list.Length;

				System.Array.Resize(ref lightPresetList.list, bufferCount);

				for(int i = oldCount; i < bufferCount; i++) {
					lightPresetList.list[i] = new LightPreset(i);
				}
			}

			for(int i = 0; i < lightPresetList.list.Length; i++) {
				bool fold = GUIFoldout.Draw( "Preset (Id: " + (i + 1) + ")" , lightPresetList.list[i]);

				if (fold == false) {
					continue;
				}

				EditorGUI.indentLevel++;

				lightPresetList.list[i].name = EditorGUILayout.TextField ("Name", lightPresetList.list[i].name);

				EditorGUILayout.Space();
				
				DrawLightLayers(lightPresetList.list[i].layerSetting);

				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	static public void DrawLightLayers(LightPresetLayers presetLayers) {
		LayerSetting[] layerSetting = presetLayers.Get();

		int layerCount = layerSetting.Length;

		layerCount = EditorGUILayout.IntSlider("Layer Count", layerCount, 1, 4);

		EditorGUILayout.Space();

		if (layerCount != layerSetting.Length) {
			int oldCount = layerSetting.Length;

			System.Array.Resize(ref layerSetting, layerCount);

			for(int i = oldCount; i < layerCount; i++) {
				
				if (layerSetting[i] == null) {
					layerSetting[i] = new LayerSetting();
					layerSetting[i].layerID = (LightingLayer)i;
				}
				
			}

			presetLayers.SetArray(layerSetting);
		}

		for(int i = 0; i < layerSetting.Length; i++) {
			LayerSetting layer = layerSetting[i];

			layer.layerID = (LightingLayer)EditorGUILayout.Popup("Layer (Id: " + (i + 1) +")", (int)layer.layerID, Lighting2D.ProjectSettings.layers.lightLayers.GetNames());
			
			layer.type = (LightingLayerType)EditorGUILayout.EnumPopup("Type", layer.type);
			
			


			layer.sorting = (LightingLayerSorting)EditorGUILayout.EnumPopup("Sorting", layer.sorting);
			
			EditorGUI.BeginDisabledGroup(layer.sorting == LightingLayerSorting.None);
			
			layer.sortingIgnore = (LightingLayerSortingIgnore)EditorGUILayout.EnumPopup("Sorting Ignore", layer.sortingIgnore);
			
			EditorGUI.EndDisabledGroup();


			layer.effect = (LightingLayerEffect)EditorGUILayout.EnumPopup("Mask Effect", layer.effect);
	
			EditorGUI.BeginDisabledGroup(layer.effect != LightingLayerEffect.AboveLit);
		
			layer.maskEffectDistance = EditorGUILayout.FloatField("Mask Effect Distance", layer.maskEffectDistance);
	
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();
		}

	}

	public class BufferPresets {
		public static void Draw(BufferPresetList bufferList) {
			bool foldout = GUIFoldoutHeader.Begin( "Buffer Presets (" + bufferList.list.Length + ")", bufferList);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

			int bufferCount = EditorGUILayout.IntSlider ("Count", bufferList.list.Length, 1, 4);

			if (bufferCount != bufferList.list.Length) {
				int oldCount = bufferList.list.Length;

				System.Array.Resize(ref bufferList.list, bufferCount);

				for(int i = oldCount; i < bufferCount; i++) {
					bufferList.list[i] = new BufferPreset(i);
				}
			}

			for(int i = 0; i < bufferList.list.Length; i++) {
				bool fold = GUIFoldout.Draw( "Preset (Id: " + (i + 1) + ")" , bufferList.list[i]);

				if (fold == false) {
					continue;
				}

				EditorGUI.indentLevel++;

				bufferList.list[i].name = EditorGUILayout.TextField ("Name", bufferList.list[i].name);

				EditorGUILayout.Space();

				CommonSettings(bufferList.list[i]);

				EditorGUILayout.Space();

				if (Lighting2D.ProjectSettings.renderingMode == RenderingMode.OnRender) {
					SortingLayer(bufferList.list[i].sortingLayer);	
				}
				
				EditorGUILayout.Space();

				LayerSettings.DrawList(bufferList.list[i].dayLayers, "Day Layers (" + bufferList.list[i].dayLayers.list.Length + ")", Lighting2D.ProjectSettings.layers.dayLayers, true);

				EditorGUILayout.Space();
				
				LayerSettings.DrawList(bufferList.list[i].nightLayers, "Night Layers (" + bufferList.list[i].nightLayers.list.Length  + ")", Lighting2D.ProjectSettings.layers.nightLayers, false);

				EditorGUILayout.Space();

				EditorGUI.indentLevel--;
			}
		
			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class QualitySettings {

		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Quality", profile.qualitySettings);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}
	
			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.qualitySettings.HDR = EditorGUILayout.Toggle("HDR", profile.qualitySettings.HDR);

				profile.qualitySettings.highQualityShadows = (LightingSettings.QualitySettings.ShadowQuality)EditorGUILayout.EnumPopup("Shadow Quality", profile.qualitySettings.highQualityShadows);

				profile.qualitySettings.updateMethod = (LightingSettings.QualitySettings.UpdateMethod)EditorGUILayout.EnumPopup("Update Method", profile.qualitySettings.updateMethod);

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class FogOfWar {
		// Fog Of War Preset????
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Fog of War", profile.fogOfWar); 

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.fogOfWar.enabled = EditorGUILayout.Toggle("Enable", profile.fogOfWar.enabled);

				profile.fogOfWar.bufferID = EditorGUILayout.Popup("Main Buffer", (int)profile.fogOfWar.bufferID, profile.bufferPresets.GetBufferLayers());
				
				SortingLayer(profile.fogOfWar.sortingLayer);			

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class DayLighting {
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Day Lighting", profile.dayLightingSettings);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.dayLightingSettings.enable = EditorGUILayout.Toggle("Enable", profile.dayLightingSettings.enable);

				profile.dayLightingSettings.alpha = EditorGUILayout.Slider("Alpha", profile.dayLightingSettings.alpha, 0, 1);

				profile.dayLightingSettings.direction = EditorGUILayout.Slider("Direction", profile.dayLightingSettings.direction, 0 , 360);

				profile.dayLightingSettings.height = EditorGUILayout.Slider("Height", profile.dayLightingSettings.height, 0, 10);

				SunPenumbra.Draw(profile);

				NormalMap.Draw(profile);
		
			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}

		public class NormalMap {
			public static void Draw(LightingSettings.Profile profile) {
				bool foldout = GUIFoldout.Draw( "Normal Map", profile.dayLightingSettings.bumpMap);

				if (foldout == false) {
					return;
				}

				EditorGUI.indentLevel++;

					profile.dayLightingSettings.bumpMap.height = EditorGUILayout.Slider("Height", profile.dayLightingSettings.bumpMap.height, 0, 5);
					profile.dayLightingSettings.bumpMap.strength = EditorGUILayout.Slider("Strength", profile.dayLightingSettings.bumpMap.strength, 0, 5);
					
				EditorGUI.indentLevel--;
			}
		}

		public class SunPenumbra {
			public static void Draw(LightingSettings.Profile profile) {
				bool foldout = GUIFoldout.Draw( "Softness", profile.dayLightingSettings.softness);

				if (foldout == false) {
					return;
				}
		
				EditorGUI.indentLevel++;	

					profile.dayLightingSettings.softness.enable = EditorGUILayout.Toggle("Enable", profile.dayLightingSettings.softness.enable);
					profile.dayLightingSettings.softness.intensity = EditorGUILayout.FloatField("Intensity", profile.dayLightingSettings.softness.intensity);
							
				EditorGUI.indentLevel--;
			}	
		}
	}

	static void CommonSettings(BufferPreset bufferPreset) {
		bufferPreset.darknessColor = EditorGUILayout.ColorField("Darkness Color", bufferPreset.darknessColor);
		bufferPreset.darknessColor.a = EditorGUILayout.Slider("Darkness Alpha", bufferPreset.darknessColor.a, 0, 1);
	
		bufferPreset.lightingResolution = EditorGUILayout.Slider("Lighting Resolution", bufferPreset.lightingResolution, 0.25f, 1.0f);
	}

	static void SortingLayer(LightingSettings.SortingLayer sortingLayer) {
		EditorGUI.BeginDisabledGroup(Lighting2D.ProjectSettings.renderingMode != RenderingMode.OnRender);

		GUISortingLayer.Draw(sortingLayer);

		EditorGUI.EndDisabledGroup();
	}

	public class LayerSettings {

		public static void DrawList(PresetLayers bufferLayers, string name, LayersList layerList, bool drawType) {
			bool foldout = GUIFoldout.Draw(name, bufferLayers);

			if (foldout == false) {
				return;
			}

			EditorGUI.indentLevel++;

			LightingLayerSetting[] layerSettings = bufferLayers.Get();
		
			int layerCount = EditorGUILayout.IntSlider ("Count", layerSettings.Length, 0, 10);

			EditorGUILayout.Space();
			
			if (layerCount != layerSettings.Length) {
				int oldCount = layerSettings.Length;

				System.Array.Resize(ref layerSettings, layerCount);

				for(int i = oldCount; i < layerCount; i++) {
					
					if (layerSettings[i] == null) {
						layerSettings[i] = new LightingLayerSetting();
						layerSettings[i].layer = (LightingLayer)i;
					}
					
				}

				bufferLayers.SetArray(layerSettings);
			}

			for(int i = 0; i < layerSettings.Length; i++) {
				layerSettings[i].layer = (LightingLayer)EditorGUILayout.Popup("Layer", (int)layerSettings[i].layer, layerList.GetNames());

				if (drawType) {
					layerSettings[i].type = (LightingLayerSettingType)EditorGUILayout.EnumPopup("Type", layerSettings[i].type);
				}
				
				layerSettings[i].sorting = (LightingLayerSettingSorting)EditorGUILayout.EnumPopup("Sorting", layerSettings[i].sorting);
				

				EditorGUILayout.Space();
			}

			EditorGUI.indentLevel--;
		}
	}
}
