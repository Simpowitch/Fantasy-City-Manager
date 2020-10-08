using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
	
    public class SpriteRenderer2D {

        static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
            List<LightingSpriteRenderer2D> spriteRendererList = LightingSpriteRenderer2D.GetList();

            Material material;

			for(int i = 0; i < spriteRendererList.Count; i++) {
				LightingSpriteRenderer2D id = spriteRendererList[i];

				if ((int)id.nightLayer != nightLayer) {
					continue;
				}

				if (id.GetSprite() == null) {
					continue;
				}

				if (id.InCamera(camera) == false) {
					continue;
				}

				Vector2 position = id.transform.position;

				Vector2 scale = id.transform.lossyScale;
				scale.x *= id.transformOffset.offsetScale.x;
				scale.y *= id.transformOffset.offsetScale.y;

				float rot = id.transformOffset.offsetRotation;
				if (id.transformOffset.applyTransformRotation) {
					rot += id.transform.rotation.eulerAngles.z;
				}

				switch(id.type) {
					case LightingSpriteRenderer2D.Type.Particle: 

						Color color = id.color;

						material = Lighting2D.materials.GetAdditive();
						material.SetColor ("_TintColor", color);

						material.mainTexture = id.GetSprite().texture;
						Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(id.spriteMeshObject, material, id.spriteRenderer, offset + position + id.transformOffset.offsetPosition, scale, rot, z);
						material.mainTexture = null;

						break;

					case LightingSpriteRenderer2D.Type.Mask:

						material = Lighting2D.materials.GetWhiteSprite();
						
						material.mainTexture = id.GetSprite().texture;
						material.color = id.color;
						Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(id.spriteMeshObject, material, id.spriteRenderer, offset + position + id.transformOffset.offsetPosition, scale, rot, z);
						material.mainTexture = null;
						material.color = Color.white;
					
						break;

	
				}
			}
		}
	}
}