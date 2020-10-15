using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class TilemapIsometric : Base {
        
        static public void MaskSprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
				return;
			}

			if (id.isometric.maskType != LightingTilemapCollider.Isometric.MaskType.Sprite) {
				return;
			}

			GL.Color(Color.white);
			
			Vector2 lightPosition = -buffer.lightSource.transform.position;
			Vector2 scale = Isometric.GetScale(id);

			MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);

			foreach(LightingTile tile in id.isometric.mapTiles) {
				virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

				Vector2 tilePosition = Isometric.GetTilePosition(tile, id);

				tilePosition += lightPosition;

				if (Vector2.Distance(Vector2.zero, tilePosition) > buffer.lightSource.size * 1.5f) {
					continue;
				}

				material.mainTexture = virtualSpriteRenderer.sprite.texture;

				Rendering.Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale, 0, z);
				
				material.mainTexture = null;
			}
		}

		// Supports only static "tile" shape
		// No support for Custom Physics Shape
		static public void MaskShape(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
				return;
			}

			if (id.isometric.maskType == LightingTilemapCollider.Isometric.MaskType.None) {
				return;
			}

			if (id.isometric.maskType == LightingTilemapCollider.Isometric.MaskType.Sprite) {
				return;
			}

			GL.Color(Color.white);

			Vector2 lightPosition = -buffer.lightSource.transform.position;
			Vector2 scale = Isometric.GetScale(id);

			MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);

			foreach(LightingTile tile in id.isometric.mapTiles) {
				List<Polygon2D> polygons = tile.GetPolygons(id);

				Vector2 tilePosition = Isometric.GetTilePosition(tile, id);		
				tilePosition.y -= 0.25f;

				tilePosition += lightPosition;

				if (Vector2.Distance(Vector2.zero, tilePosition) > buffer.lightSource.size * 1.5f) {
					continue;
				}

				GLExtended.DrawMesh(tileMesh, tilePosition, scale, 0);
			}
		}
    }
}