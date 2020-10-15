using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
        
    public class TextureRenderer  {
        
		public static void Draw(LightingTextureRenderer2D id , Camera camera, Vector2 offset, float z) {
			if (id.InCamera(camera) == false) {
				return;
			}

			Material material = Lighting2D.materials.GetAdditive();
			material.SetColor ("_TintColor", id.color);

			material.mainTexture = id.texture;

			Rendering.Universal.WithoutAtlas.Texture.Draw(material, new Vector3(offset.x, offset.y) + id.transform.position, id.size, 0, z);
			
			material.mainTexture = null;
		}
    }
}