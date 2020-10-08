using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class TilemapIsometric : Base {
        
        static public void MaskSprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
				return;
			}

			if (id.maskType != LightingTilemapCollider2D.MaskType.Sprite) {
				return;
			}

			if (id.isometricMap == null) {
				return;
			}

			MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);

			Vector2 offset = -buffer.lightSource.transform.position;

			GL.Color(Color.white);

			Vector2 tilePosition;
			Vector2 polyOffset;

			foreach(LightingTilemapCollider2D.IsometricTile tile in id.isometricMap.mapTiles) {
				virtualSpriteRenderer.sprite = tile.tile.GetOriginalSprite();

				tilePosition = Vector2.zero;

				tilePosition.y += 0.5f * id.properties.cellSize.y;

				tilePosition.x += tile.position.x * 0.5f;
				tilePosition.y += tile.position.x * 0.5f * id.properties.cellSize.y;

				tilePosition.x += tile.position.y * -0.5f;
				tilePosition.y += tile.position.y * 0.5f * id.properties.cellSize.y;

				tilePosition.x *= id.properties.cellSize.x;

				polyOffset.x = offset.x + tilePosition.x;
				polyOffset.y = offset.y + tilePosition.y;

				material.mainTexture = virtualSpriteRenderer.sprite.texture;

				if (Vector2.Distance(Vector2.zero, polyOffset) > buffer.lightSource.size * 1.5f) {
					continue;
				}

				Rendering.Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.tile.spriteMeshObject, material, virtualSpriteRenderer, polyOffset, Vector2.one, 0, z);
				
				material.mainTexture = null;
			}
		}

		// Supports only static "tile" shape
		// No support for Custom Physics Shape
		static public void MaskShape(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
				return;
			}

			if (id.maskType == LightingTilemapCollider2D.MaskType.None) {
				return;
			}

			if (id.maskType == LightingTilemapCollider2D.MaskType.Sprite) {
				return;
			}
			
			if (id.isometricMap == null) {
				return;
			}

			Vector2 vecA, vecB, vecC;

			Vector2 scale = Vector2.one;

			MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);
			Vector2 offset = -buffer.lightSource.transform.position;
			Vector2 polyOffset;
			
			GL.Color(Color.white);

			foreach(LightingTilemapCollider2D.IsometricTile tile in id.isometricMap.mapTiles) {
				polygons = tile.tile.GetPolygons(id);

				polyOffset.x = offset.x;
				polyOffset.y = offset.y;

				polyOffset.y += 0.5f * id.properties.cellSize.y;

				polyOffset.x += tile.position.x * 0.5f;
				polyOffset.y += tile.position.x * 0.5f * id.properties.cellSize.y;

				polyOffset.x += tile.position.y * -0.5f ;
				polyOffset.y += tile.position.y * 0.5f * id.properties.cellSize.y;

				if (id.colliderType == LightingTilemapCollider2D.ColliderType.SpriteCustomPhysicsShape) {
					polyOffset.y += 0.25f;
				}

				if (Vector2.Distance(Vector2.zero, polyOffset) > buffer.lightSource.size * 1.5f) {
					continue;
				}

				// Batch and Optimize???
				int triangleCount = tileMesh.triangles.GetLength (0);
				for (int i = 0; i < triangleCount; i = i + 3) {
					vecA = tileMesh.vertices [tileMesh.triangles [i]];
					vecB = tileMesh.vertices [tileMesh.triangles [i + 1]];
					vecC = tileMesh.vertices [tileMesh.triangles [i + 2]];

					Max2D.DrawTriangle(vecA, vecB, vecC, polyOffset, scale, z);
				}
			}
		}
    }
}