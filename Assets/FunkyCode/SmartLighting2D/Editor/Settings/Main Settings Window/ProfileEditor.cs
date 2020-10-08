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

		if (profile == null) {
			EditorGUILayout.HelpBox("Lighting2D Settings Profile Not Found!", MessageType.Error);

			return;
		}

		CommonSettings(profile.bufferPresets.list[0]);

		SortingLayer(profile.bufferPresets.list[0].sortingLayer);

		QualitySettings.Draw(profile);

		BufferPresets.Draw(profile.bufferPresets);

		DayLighting.Draw(profile);
		
		FogOfWar.Draw(profile);

		profile.disable = EditorGUILayout.Toggle("Disable", profile.disable);

		EditorGUI.EndChangeCheck ();

		if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {
				LightingManager2D.ForceUpdate();

				EditorUtility.SetDirty(profile);
			}
		}
	}

	public class BufferPresets {
		public static void Draw(BufferPresetList bufferList) {
			bool foldout = GUIFoldout.Draw( "Buffer Presets", bufferList);

			if (foldout == false) {
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

				if (Lighting2D.ProjectSettings.renderingMode == RenderingMode.OnRender) {
					SortingLayer(bufferList.list[i].sortingLayer);	
				}

				CommonSettings(bufferList.list[i]);
				
				LayerSettings.DrawList(bufferList.list[i].dayLayers, "Day Layers", Lighting2D.ProjectSettings.layers.dayLayers);
				LayerSettings.DrawList(bufferList.list[i].nightLayers, "Night Layers", Lighting2D.ProjectSettings.layers.nightLayers);

				EditorGUI.indentLevel--;
			}
		
			EditorGUI.indentLevel--;
		}
	}

	public class QualitySettings {

		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldout.Draw( "Quality", profile.qualitySettings);

			if (foldout == false) {
				return;
			}
	
			EditorGUI.indentLevel++;

				profile.qualitySettings.HDR = EditorGUILayout.Toggle("HDR", profile.qualitySettings.HDR);

				profile.qualitySettings.highQualityShadows = (LightingSettings.QualitySettings.ShadowQuality)EditorGUILayout.EnumPopup("Shadow Quality", profile.qualitySettings.highQualityShadows);

				profile.qualitySettings.updateMethod = (LightingSettings.QualitySettings.UpdateMethod)EditorGUILayout.EnumPopup("Update Method", profile.qualitySettings.updateMethod);

			EditorGUI.indentLevel--;
		}
	}

	public class FogOfWar {
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldout.Draw( "Fog of War", profile.fogOfWar);

			if (foldout == false) {
				return;
			}

			EditorGUI.indentLevel++;

				profile.fogOfWar.enabled = EditorGUILayout.Toggle("Enable", profile.fogOfWar.enabled);

				profile.fogOfWar.bufferID = EditorGUILayout.Popup("Main Buffer", (int)profile.fogOfWar.bufferID, profile.bufferPresets.GetBufferLayers());
				
				SortingLayer(profile.fogOfWar.sortingLayer);			

			EditorGUI.indentLevel--;
		}
	}

	public class DayLighting {
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldout.Draw( "Day Lighting", profile.dayLightingSettings);

			if (foldout == false) {
				return;
			}

			EditorGUI.indentLevel++;	

				profile.dayLightingSettings.enable = EditorGUILayout.Toggle("Enable", profile.dayLightingSettings.enable);

				profile.dayLightingSettings.alpha = EditorGUILayout.Slider("Alpha", profile.dayLightingSettings.alpha, 0, 1);

				profile.dayLightingSettings.direction = EditorGUILayout.Slider("Direction", profile.dayLightingSettings.direction, 0 , 360);

				profile.dayLightingSettings.height = EditorGUILayout.Slider("Height", profile.dayLightingSettings.height, 0, 10);

				SunPenumbra.Draw(profile);

				NormalMap.Draw(profile);
		
			EditorGUI.indentLevel--;
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

		public static void DrawList(PresetLayers bufferLayers, string name, LayersList layerList) {
			bool foldout = GUIFoldout.Draw(name, bufferLayers);

			if (foldout == false) {
				return;
			}

			EditorGUI.indentLevel++;
		
			int layerCount = EditorGUILayout.IntSlider ("Count", bufferLayers.list.Length, 1, 10);
			
			if (layerCount != bufferLayers.list.Length && layerCount > 0) {
				int oldCount = bufferLayers.list.Length;

				System.Array.Resize(ref bufferLayers.list, layerCount);

				for(int i = oldCount; i < layerCount; i++) {
					bufferLayers.list[i] = (LightingLayer)i;
				}
			}

			for(int i = 0; i < bufferLayers.list.Length; i++) {
				bufferLayers.list[i] = (LightingLayer)EditorGUILayout.Popup(" ", (int)bufferLayers.list[i], layerList.GetNames());
			}

			EditorGUI.indentLevel--;
		}
	}
}
