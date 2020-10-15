using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class TilemapRectangle : Rendering.Light.Base {

        static public void Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
				return;
			}

			if (id.rectangle.maskType != LightingTilemapCollider.Rectangle.MaskType.Sprite) {
				return;
			}
			
			if (id.rectangle.map == null) {
				return;
			}

			TilemapProperties properties = id.rectangle.Properties;
			Vector2 offset = -buffer.lightSource.transform.position;

			int tilemapSize = Rectangle.Light.GetSize(id, buffer);
			Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
					if (x < 0 || y < 0 || x >= properties.arraySize.x || y >= properties.arraySize.y) {
						continue;
					}

					LightingTile tile = id.rectangle.map.map[x, y];
					if (tile == null) {
						continue;
					}

					if (tile.GetOriginalSprite() == null) {
						continue;
					}

                    Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

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
}
