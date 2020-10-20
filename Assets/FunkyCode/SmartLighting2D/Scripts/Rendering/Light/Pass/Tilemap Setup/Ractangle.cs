using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

   public class Rectangle {

       static public Vector2 GetTilePosition(int x, int y, Transform transform, TilemapProperties properties) {
            Vector2 tilemapOffset = transform.position;
            Vector2 tilePosition = new Vector2(x, y);
        
            // Cell Gap
            // tilePosition.x *= (properties.cellGap.x / properties.cellSize.x + 1);
            // tilePosition.y *= (properties.cellGap.y / properties.cellSize.y + 1);

            // Tilemap Anchor
            tilePosition.x += properties.cellAnchor.x;
            tilePosition.y += properties.cellAnchor.y;

            // Tilemap Array Offset
            tilePosition.x += properties.area.position.x;
            tilePosition.y += properties.area.position.y;

            // Scale Offset
            tilePosition.x *= transform.lossyScale.x;
            tilePosition.y *= transform.lossyScale.y;

            // Tilemap Rotation Swap            
            Vector2 rotScale = GetRotationScale(transform.rotation);
            tilePosition *= rotScale;

            // Tilemap Cell Size
            tilePosition.x *= properties.cellSize.x;
            tilePosition.y *= properties.cellSize.y;

            // Tilemap Offset
            tilePosition += tilemapOffset;

            return(tilePosition);
        }

        static public Vector2 GetTilePosition(int x, int y, LightingTilemapCollider2D id) {
            TilemapProperties properties = id.rectangle.Properties;

            Vector2 tilemapOffset = id.transform.position;
            Vector2 tilePosition = new Vector2(x, y);
        
            // Cell Gap
            // tilePosition.x *= (properties.cellGap.x / properties.cellSize.x + 1);
            // tilePosition.y *= (properties.cellGap.y / properties.cellSize.y + 1);

            // Tilemap Anchor
            tilePosition.x += properties.cellAnchor.x;
            tilePosition.y += properties.cellAnchor.y;

            // Tilemap Array Offset
            tilePosition.x += properties.area.position.x;
            tilePosition.y += properties.area.position.y;

            // Scale Offset
            tilePosition.x *= id.transform.lossyScale.x;
            tilePosition.y *= id.transform.lossyScale.y;

            // Tilemap Rotation Swap            
            Vector2 rotScale = GetRotationScale(id.transform.rotation);
            tilePosition *= rotScale;

            // Tilemap Cell Size
            tilePosition.x *= properties.cellSize.x;
            tilePosition.y *= properties.cellSize.y;

            // Tilemap Offset
            tilePosition += tilemapOffset;

            return(tilePosition);
        }

         public static Vector2 GetScale(LightingTilemapCollider2D id, bool isGrid) {
            Vector2 scale = Vector2.one;

            TilemapProperties properties = id.GetTilemapProperties();

            scale.x *= id.transform.lossyScale.x; 
            scale.y *= id.transform.lossyScale.y;

            if (isGrid) {
                scale.x *= properties.cellSize.x;
                scale.y *= properties.cellSize.y;
            }

            return(scale);
        }

        // Tilemap
        private static Vector2 GetRotationScale(Quaternion rotation) {
            Vector3 rot = Math2D.GetPitchYawRollRad(rotation);

            Vector2 rotScale;
            rotScale.x = Mathf.Sin(rot.y + Mathf.PI / 2);
            rotScale.y = Mathf.Sin(rot.x + Mathf.PI / 2);

            return(rotScale);
        }

        public class Light {
            static public int GetSize(LightingTilemapCollider2D id, LightingBuffer2D buffer) {
                Vector2 rotScale = GetRotationScale(id.transform.rotation);

                TilemapProperties properties = id.GetTilemapProperties();
            
                float sx = 1f;
                sx /= properties.cellSize.x;
                sx /= id.transform.lossyScale.x;
                sx /= rotScale.x;

                float sy = 1f;
                sy /= properties.cellSize.y;
                sy /= id.transform.localScale.y;
                sy /= rotScale.y;

                float size = buffer.lightSource.size + 1;
                size *= Mathf.Max(sx, sy);

                return((int) size);
            }

            static public Vector2Int GetPosition(LightingTilemapCollider2D id, LightingBuffer2D buffer) {
                Vector2 newPosition = Vector2.zero;
                newPosition.x = buffer.lightSource.transform.position.x;
                newPosition.y = buffer.lightSource.transform.position.y;

                Vector2 rotScale = GetRotationScale(id.transform.rotation);

                TilemapProperties properties = id.GetTilemapProperties();

                float sx = 1; 
                sx /= properties.cellSize.x;
                sx /= id.transform.lossyScale.x;
                sx /= rotScale.x;


                float sy = 1;
                sy /= properties.cellSize.y;
                sy /= id.transform.lossyScale.y;
                sy /= rotScale.y;

                newPosition.x *= sx;
                newPosition.y *= sy;

                Vector2 tilemapPosition = Vector2.zero;

                tilemapPosition.x -= properties.area.position.x;
                tilemapPosition.y -= properties.area.position.y;
                
                tilemapPosition.x -= id.transform.position.x;
                tilemapPosition.y -= id.transform.position.y;
                    
                tilemapPosition.x -= properties.cellAnchor.x;
                tilemapPosition.y -= properties.cellAnchor.y;

                // Cell Size Is Not Calculated Correctly
                tilemapPosition.x += 1;
                tilemapPosition.y += 1;
                
                newPosition.x += tilemapPosition.x;
                newPosition.y += tilemapPosition.y;

                return(new Vector2Int((int)newPosition.x, (int)newPosition.y));
            }
        }
    }
}