using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public class Isometric {

        static public Vector2 GetTilePosition(LightingTile tile, LightingTilemapCollider2D id) {
           TilemapProperties properties = id.isometric.Properties;
           
            Vector2 tilemapOffset = id.transform.position;
            
            // Tile Offset
            Vector2 tileOffset = new Vector2(tile.position.x, tile.position.y);
            tileOffset.x += properties.cellAnchor.x;
            tileOffset.y += properties.cellAnchor.y;

            tileOffset.x += properties.cellGap.x * tile.position.x;
            tileOffset.y += properties.cellGap.y * tile.position.y;
            
            // Tile Position
            Vector2 tilePosition = tilemapOffset;
            
            tilePosition.x += tileOffset.x * 0.5f;
            tilePosition.x += tileOffset.y * -0.5f;
            tilePosition.x *= properties.cellSize.x;

            tilePosition.y += tileOffset.x * 0.5f * properties.cellSize.y;
            tilePosition.y += tileOffset.y * 0.5f * properties.cellSize.y;
            
            tilePosition.x *= id.transform.lossyScale.x;
            tilePosition.y *= id.transform.lossyScale.y;

            //tilePosition.x *= (properties.cellGap.x + properties.cellSize.x);
            //tilePosition.y *= (properties.cellGap.y + properties.cellSize.y);
            
            return(tilePosition);
        }
       
        public static Vector2 GetScale(LightingTilemapCollider2D id) {
            Vector2 scale = new Vector2();

            // TilemapProperties properties = id.GetTilemapProperties();

            scale.x = id.transform.lossyScale.x; //properties.cellSize.x * 
            scale.y = id.transform.lossyScale.y; // properties.cellSize.y *

            return(scale);
        }

    }

}