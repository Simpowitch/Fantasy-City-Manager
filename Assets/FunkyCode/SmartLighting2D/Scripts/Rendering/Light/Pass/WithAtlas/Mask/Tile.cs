using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class Tile {
		public static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
        
		static public void MaskSprite(LightingBuffer2D buffer, LightingTile tile, LayerSetting layerSetting, LightingTilemapCollider2D id, Vector2 offset, float z) {
			if (id.rectangle.maskType != LightingTilemapCollider.Rectangle.MaskType.Sprite) {
				return;
			}

			if (id.rectangle.maskType == LightingTilemapCollider.Rectangle.MaskType.None) {
				return;
			}

			if (tile.GetOriginalSprite() == null) {
				return;
			}

			UnityEngine.Sprite sprite = tile.GetAtlasSprite();

			if (sprite == null) {
				UnityEngine.Sprite reqSprite = AtlasSystem.Manager.RequestSprite(tile.GetOriginalSprite(), AtlasSystem.Request.Type.WhiteMask);
				if (reqSprite == null) {
					PartiallyBatchedTilemap batched = new PartiallyBatchedTilemap();

					batched.virtualSpriteRenderer = new VirtualSpriteRenderer();
					batched.virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

					batched.polyOffset = offset;

					batched.tileSize = id.transform.lossyScale;

					batched.tilemap = id;

					buffer.lightingAtlasBatches.tilemapList.Add(batched);
					return;
				} else {
					tile.SetAtlasSprite(reqSprite);
					sprite = reqSprite;
				}
			}

			virtualSpriteRenderer.sprite = sprite;

			Rendering.Universal.WithAtlas.Sprite.Draw(virtualSpriteRenderer, layerSetting, MaskEffect.Lit, offset, id.transform.lossyScale, id.transform.rotation.eulerAngles.z, z);	
		}
    }
}