using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class Main {

		public static void Draw(Camera camera, Vector2 offset, float z, BufferPreset bufferPreset) {
			DarknessColor(camera, z, bufferPreset);

			LightingLayerSetting[] layerSettings = bufferPreset.nightLayers.Get();

			if (layerSettings.Length > 0) {

				for(int i = 0; i < layerSettings.Length; i++) {
					LightingLayerSetting nightLayer = layerSettings[i];

					LightingLayerSettingSorting sorting = nightLayer.sorting;

					if (sorting == LightingLayerSettingSorting.None) {
						NoSort.Draw(camera, offset, z, nightLayer);
					} else {
						Sorted.Draw(camera, offset, z, nightLayer);
					}
				}

			}
		}

		public static void DarknessColor(Camera camera, float z, BufferPreset bufferPreset) {
			Color color = bufferPreset.darknessColor;

			if (color.a > 0) {
				Material material = Lighting2D.materials.GetAlphaBlend();		
				material.SetColor ("_TintColor", color);
				material.mainTexture = null;

				Rendering.Universal.WithoutAtlas.Texture.Draw(material, Vector2.zero, LightingRender2D.GetSize(camera), camera.transform.eulerAngles.z,z);
			}
		}		
	}
}