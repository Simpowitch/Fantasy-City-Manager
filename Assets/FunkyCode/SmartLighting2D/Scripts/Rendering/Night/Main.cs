using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class Main {

		public static void Draw(Camera camera, Vector2 offset, float z, BufferPreset bufferPreset) {
			DarknessColor(camera, z, bufferPreset);

			for(int i = 0; i < bufferPreset.nightLayers.list.Length; i++) {
				int nightLayer = (int)bufferPreset.nightLayers.list[i];

				if (Lighting2D.atlasSettings.lightingSpriteAtlas) {
					WithAtlas.SpriteRenderer.Draw(camera, offset, z, nightLayer);
					
				} else {
					WithoutAtlas.Room.Draw(camera, offset, z, nightLayer);

					#if UNITY_2017_4_OR_NEWER
						WithoutAtlas.TilemapRoom.Draw(camera, offset, z, nightLayer);
					#endif
			
					WithoutAtlas.SpriteRenderer2D.Draw(camera, offset, z, nightLayer);
					WithoutAtlas.TextureRenderer.Draw(camera, offset, z, nightLayer);
					WithoutAtlas.ParticleRenderer.Draw(camera, offset, z, nightLayer);
				}
			}

			LightBuffers.Draw(camera, offset, z, bufferPreset);
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