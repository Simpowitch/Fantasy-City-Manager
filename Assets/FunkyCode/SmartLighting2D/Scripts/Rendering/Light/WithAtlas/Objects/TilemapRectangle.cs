using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class TilemapRectangle : Rendering.Light.Base {

        static public void Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
				return;
			}

			if (id.maskType != LightingTilemapCollider2D.MaskType.Sprite) {
				return;
			}
			
			if (id.rectangleMap.map == null) {
				return;
			}

			Vector2 positionScale = GetPositionScale(id);
			Vector2 tilemapOffset = GetTilemapOffset(id);
			int tilemapSize = GetTilemapSize(id, buffer);
			Vector2 offset = -buffer.lightSource.transform.position;
			Vector2Int tilemapLightPosition = GetTilemapLightPosition(id, buffer);

            Vector2 polyOffset;

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
					if (x < 0 || y < 0 || x >= id.properties.arraySize.x || y >= id.properties.arraySize.y) {
						continue;
					}

					LightingTile tile = id.rectangleMap.map[x, y];
					if (tile == null) {
						continue;
					}

					if (tile.GetOriginalSprite() == null) {
						continue;
					}

					polyOffset.x = (x + tilemapOffset.x) * positionScale.x;
					polyOffset.y = (y + tilemapOffset.y) * positionScale.y;

					if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
						continue;
					}

					polyOffset += offset;
					
					virtualSpriteRenderer.sprite = tile.GetAtlasSprite();

					if (virtualSpriteRenderer.sprite == null) {
						reqSprite = AtlasSystem.Manager.RequestSprite(tile.GetOriginalSprite(), AtlasSystem.Request.Type.WhiteMask);
						if (reqSprite == null) {
							// Add Partialy Batched
							batched = new PartiallyBatchedTilemap();

							batched.virtualSpriteRenderer = new VirtualSpriteRenderer();
							batched.virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

							batched.polyOffset = polyOffset;
							batched.tile = tile;

							batched.tileSize = id.transform.lossyScale;

							buffer.lightingAtlasBatches.tilemapList.Add(batched);
							continue;
						} else {
							tile.SetAtlasSprite(reqSprite);
							virtualSpriteRenderer.sprite = reqSprite;
						}
					}

					Rendering.Universal.WithAtlas.Sprite.Draw(virtualSpriteRenderer, buffer.lightSource.layerSetting[0], MaskEffect.Lit, polyOffset, id.transform.lossyScale, 0, z);
				}	
			}
		}
    }
}
