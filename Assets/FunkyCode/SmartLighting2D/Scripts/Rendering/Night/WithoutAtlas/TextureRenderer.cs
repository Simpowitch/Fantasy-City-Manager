using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
        
    public class TextureRenderer  {
        
		static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
            List<LightingTextureRenderer2D> textureRendererList= LightingTextureRenderer2D.GetList();

			for(int i = 0; i < textureRendererList.Count; i++) {
				LightingTextureRenderer2D id = textureRendererList[i];

				if ((int)id.nightLayer != nightLayer) {
					continue;
				}

				if (id.InCamera(camera) == false) {
					continue;
				}

				Material material = Lighting2D.materials.GetAdditive();
				material.SetColor ("_TintColor", id.color);

				material.mainTexture = id.texture;

				Rendering.Universal.WithoutAtlas.Texture.Draw(material, new Vector3(offset.x, offset.y) + id.transform.position, id.size, 0, z);
				
				material.mainTexture = null;
			}
		}
    }
}