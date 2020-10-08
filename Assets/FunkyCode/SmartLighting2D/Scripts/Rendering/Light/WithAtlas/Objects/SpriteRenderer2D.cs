using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class SpriteRenderer2D : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

			UnityEngine.SpriteRenderer spriteRenderer = id.shape.spriteShape.GetSpriteRenderer();

			if (id.shape.spriteShape.GetOriginalSprite() == null || spriteRenderer == null) {
				return;
			}

			Sprite sprite = id.shape.spriteShape.GetAtlasSprite();
			if (sprite == null) {
				Sprite reqSprite = AtlasSystem.Manager.RequestSprite(id.shape.spriteShape.GetOriginalSprite(), AtlasSystem.Request.Type.WhiteMask);
				if (reqSprite == null) {
					PartiallyBatchedCollider batched = new PartiallyBatchedCollider();

					batched.collider = id;

					buffer.lightingAtlasBatches.colliderList.Add(batched);
					return;
				} else {
					id.shape.spriteShape.SetAtlasSprite(reqSprite);
					sprite = reqSprite;
				}
			}

			Vector2 position = id.transform2D.position - buffer.lightSource.transform2D.position;
	
			virtualSpriteRenderer.sprite = sprite;

			Rendering.Universal.WithAtlas.Sprite.Draw(virtualSpriteRenderer, buffer.lightSource.layerSetting[0], id.maskEffect, position, id.transform2D.scale, id.transform2D.rotation, z);
		}
	}
}