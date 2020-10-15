using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class LightSource {

       static public void Draw(LightingSource2D id, Camera camera, Vector2 offset, float z) {
            if (id.Buffer == null) {
                return;
            }

            if (id.isActiveAndEnabled == false) {
                return;
            }

            if (id.InAnyCamera() == false) {
                return;
            }

            Vector2 pos = id.transform2D.position + offset;
            Vector2 size = new Vector2(id.size, id.size);

            Color lightColor = id.color;
            lightColor.a = id.color.a / 2;

            Material material = Lighting2D.materials.GetAdditive();
            material.mainTexture = id.Buffer.renderTexture.renderTexture;
            material.SetColor ("_TintColor", lightColor);
        
            Rendering.Universal.WithoutAtlas.Texture.Draw(material, pos, size, z);
        }
    }
}