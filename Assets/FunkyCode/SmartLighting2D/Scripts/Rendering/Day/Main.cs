using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Day {
	
	public class Main {

		public static void Draw(Camera camera, Vector2 offset, float z, BufferPreset bufferPreset) {
			LightingLayerSetting[] layerSettings = bufferPreset.dayLayers.Get();

			for(int i = 0; i < layerSettings.Length; i++) {
				LightingLayerSetting dayLayer = layerSettings[i];

				LightingLayerSettingSorting sorting = dayLayer.sorting;

				if (sorting == LightingLayerSettingSorting.None) {
					NoSort.Draw(camera, offset, z, dayLayer);
				} else {
					Sorted.Draw(camera, offset, z, dayLayer);
				}
			}
			
			ShadowDarkness(camera, z);
		}

		static void ShadowDarkness(Camera camera, float z) {
			if (Lighting2D.dayLightingSettings.enable == false) {
				return;
			}

			Color color = new Color(0, 0, 0,  (1f - Lighting2D.dayLightingSettings.alpha) / 2);

			if (color.a > 0) {
				color.r = 0.5f;
				color.g = 0.5f;
				color.b = 0.5f;
					
				Material material = Lighting2D.materials.GetAlphaBlend();
				material.mainTexture = null;		
				material.SetColor ("_TintColor", color);

				Rendering.Universal.WithoutAtlas.Texture.Draw(material, Vector2.zero, LightingRender2D.GetSize(camera), camera.transform.eulerAngles.z, z);
			}
		}
	}
}