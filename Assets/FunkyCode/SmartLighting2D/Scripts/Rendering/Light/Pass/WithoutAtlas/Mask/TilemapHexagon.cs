using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class TilemapHexagon {

        // Supports only static "tile" shape
		// No support for Custom Physics Shape
		static public void MaskShape(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
			if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon) {
				return;
			}

			if (id.hexagon.maskType == LightingTilemapCollider.Hexagon.MaskType.None) {
				return;
			}

			if (id.hexagon.maskType == LightingTilemapCollider.Hexagon.MaskType.Sprite) {
				return;
			}

			GL.Color(Color.white);

			Vector2 lightPosition = -buffer.lightSource.transform.position;
			Vector2 scale = Hexagon.GetScale(id);
			
			MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);
			
			foreach(LightingTile tile in id.hexagon.mapTiles) {
				List<Polygon2D> polygons = tile.GetPolygons(id);

				Vector2 tilePosition = Hexagon.GetTilePosition(tile, id);		
			
				tilePosition += lightPosition;

				if (Vector2.Distance(Vector2.zero, tilePosition) > buffer.lightSource.size * 1.5f) {
					continue;
				}

				GLExtended.DrawMesh(tileMesh, tilePosition, scale, 0);
			}
		}
        
    }
}