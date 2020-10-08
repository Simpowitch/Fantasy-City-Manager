using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class LightBuffers {

        public static void Draw(Camera camera, Vector2 offset, float z, BufferPreset bufferPreset) {
            Material material = Lighting2D.materials.GetAdditive();

            bool[] nightLayers = bufferPreset.nightLayers.GetLayersArray();

            foreach (LightingSource2D id in LightingSource2D.GetList()) {
                int nightLayer = (int)id.nightLayer;

                if (nightLayers[nightLayer] == false) {
                    continue;
                }

                if (id.Buffer == null) {
                    continue;
                }

                if (id.isActiveAndEnabled == false) {
                    continue;
                }

                if (id.InAnyCamera() == false) {
                    continue;
                }

                Vector2 pos = id.transform2D.position + offset;
                Vector2 size = new Vector2(id.size, id.size);

                Color lightColor = id.color;
                lightColor.a = id.color.a / 2;

                material.mainTexture = id.Buffer.renderTexture;

                material.SetColor ("_TintColor", lightColor);
            
                Rendering.Universal.WithoutAtlas.Texture.Draw(material, pos, size, z);
            }
        }
    }
}