using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
   public class Hexagon {

        static public Vector2 GetTilePosition(LightingTile tile, LightingTilemapCollider2D id) {
            TilemapProperties properties = id.hexagon.Properties;
            
            int tx = tile.position.x + tile.position.y / 2;
            int ty = tile.position.y;

            Vector2 tileOffset = new Vector2(tx, ty);
            tileOffset.x += properties.cellAnchor.x;
            tileOffset.y += properties.cellAnchor.y;


            Vector2 tilemapOffset = id.transform.position;
            
            Vector2 tilePosition = Vector2.zero;

            tilePosition.x += tileOffset.x + tileOffset.y * -0.5f;
            tilePosition.y += tileOffset.y * 0.75f;

            tilePosition.x *= id.transform.lossyScale.x;
            tilePosition.y *= id.transform.lossyScale.y;

            tilePosition += tilemapOffset;

    
            return(tilePosition);
        }

        public static Vector2 GetScale(LightingTilemapCollider2D id) {
            Vector2 scale = new Vector2();

            TilemapProperties properties = id.GetTilemapProperties();

            scale.x = properties.cellSize.x * id.transform.lossyScale.x;
            scale.y = properties.cellSize.y * id.transform.lossyScale.y;

            return(scale);
        }

    }

}