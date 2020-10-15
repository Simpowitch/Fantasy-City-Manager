using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class SpriteRenderer2D : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

            foreach(LightingColliderShape shape in id.shapes) {
				UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

				if (shape.spriteShape.GetOriginalSprite() == null || spriteRenderer == null) {
					continue;
				}

				Sprite sprite = shape.spriteShape.GetAtlasSprite();
				if (sprite == null) {
					Sprite reqSprite = AtlasSystem.Manager.RequestSprite(shape.spriteShape.GetOriginalSprite(), AtlasSystem.Request.Type.WhiteMask);
					if (reqSprite == null) {
						PartiallyBatchedCollider batched = new PartiallyBatchedCollider();

						batched.collider = id;

						buffer.lightingAtlasBatches.colliderList.Add(batched);
						continue;
					} else {
						shape.spriteShape.SetAtlasSprite(reqSprite);
						sprite = reqSprite;
					}
				}

				Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;
		
				virtualSpriteRenderer.sprite = sprite;

				LayerSetting[] layerSettings = buffer.lightSource.GetLayerSettings();

				Rendering.Universal.WithAtlas.Sprite.Draw(virtualSpriteRenderer, layerSettings[0], id.maskEffect, position, shape.transform2D.scale, shape.transform2D.rotation, z);
			}
		}
	}
}