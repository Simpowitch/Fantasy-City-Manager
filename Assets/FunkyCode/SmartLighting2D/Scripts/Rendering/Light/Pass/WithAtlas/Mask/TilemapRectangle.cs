using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class TilemapRectangle {

		public static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

        static public void Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
				return;
			}

			if (id.rectangle.maskType != LightingTilemapCollider.Rectangle.MaskType.Sprite) {
				return;
			}
			
			Vector2 offset = -buffer.lightSource.transform.position;

            foreach(LightingTile tile in id.rectangle.mapTiles) {

				if (tile.GetOriginalSprite() == null) {
					continue;
				}

				Vector2 tilePosition = Rectangle.GetTilePosition(tile.position.x, tile.position.y, id);

				tilePosition += offset;

				if (tile.InRange(tilePosition, 2 + buffer.lightSource.size)) {
					continue;
				}
				
				virtualSpriteRenderer.sprite = tile.GetAtlasSprite();

				if (virtualSpriteRenderer.sprite == null) {
					Sprite reqSprite = AtlasSystem.Manager.RequestSprite(tile.GetOriginalSprite(), AtlasSystem.Request.Type.WhiteMask);
					if (reqSprite == null) {
						// Add Partialy Batched
						PartiallyBatchedTilemap batched = new PartiallyBatchedTilemap();

						batched.virtualSpriteRenderer = new VirtualSpriteRenderer();
						batched.virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

						batched.polyOffset = tilePosition;
						batched.tile = tile;

						batched.tileSize = id.transform.lossyScale;

						buffer.lightingAtlasBatches.tilemapList.Add(batched);
						continue;
					} else {
						tile.SetAtlasSprite(reqSprite);
						virtualSpriteRenderer.sprite = reqSprite;
					}
				}

				LayerSetting[] layerSettings = buffer.lightSource.GetLayerSettings();

				Rendering.Universal.WithAtlas.Sprite.Draw(virtualSpriteRenderer, layerSettings[0], MaskEffect.Lit, tilePosition, id.transform.lossyScale, 0, z);
			}	

		}
    }
}
