using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
	
    public class SpriteRenderer2D {

		static public void Draw(LightingSpriteRenderer2D id, Camera camera, Vector2 offset, float z) {
			Material material;

			if (id.GetSprite() == null) {
				return;
			}

			if (id.InCamera(camera) == false) {
				return;
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

					material = Lighting2D.materials.GetSpriteMask();
					
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