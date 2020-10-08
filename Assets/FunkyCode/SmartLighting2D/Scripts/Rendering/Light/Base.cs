using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
	
	public class Base {
		public static List<Polygon2D> polygons;
		public static Polygon2D polygon = null;

		public static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

		public static Sprite reqSprite;
		public static PartiallyBatchedTilemap batched;

		public static Pair2D pair = Pair2D.Zero();

        // Tilemap
        public static Vector2 GetRotationScale(Quaternion rotation) {
            Vector3 rot = Math2D.GetPitchYawRollRad(rotation);

            Vector2 rotScale;
            rotScale.x = Mathf.Sin(rot.y + Mathf.PI / 2);
            rotScale.y = Mathf.Sin(rot.x + Mathf.PI / 2);

            return(rotScale);
        }

        public static Vector2 GetPositionScale(LightingTilemapCollider2D id) {
            Vector3 rotScale = GetRotationScale(id.transform.rotation);

            Vector2 posScale;
            posScale.x = id.properties.cellSize.x * id.transform.lossyScale.x * rotScale.x;
            posScale.y = id.properties.cellSize.y * id.transform.lossyScale.y * rotScale.y;

            return(posScale);
        }

        public static Vector2 GetScale(LightingTilemapCollider2D id) {
            Vector2 scale = new Vector2();
            scale.x = id.properties.cellSize.x * id.transform.lossyScale.x;
            scale.y = id.properties.cellSize.y * id.transform.lossyScale.x;
            return(scale);
        }

        static public Vector2 GetTilemapOffset(LightingTilemapCollider2D id) {
            Vector2 tilemapOffset;
            tilemapOffset.x = id.transform.position.x + id.properties.area.position.x + id.properties.cellAnchor.x;
            tilemapOffset.y = id.transform.position.y + id.properties.area.position.y + id.properties.cellAnchor.y;
            return(tilemapOffset);
        }

        static public int GetTilemapSize(LightingTilemapCollider2D id, LightingBuffer2D buffer) {
            Vector3 rot = Math2D.GetPitchYawRollRad(id.transform.rotation);

            float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);
            float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
        
            float sx = 1f;
            sx /= id.properties.cellSize.x;
            sx /= id.transform.lossyScale.x;
            sx /= rotationXScale;

            float sy = 1f;
            sy /= id.properties.cellSize.y;
            sy /= id.transform.localScale.y;
            sy /= rotationYScale;

            float size = buffer.lightSource.size + 1;
            size *= Mathf.Max(sx, sy);

            return((int) size);
        }

        static public Vector2Int GetTilemapLightPosition(LightingTilemapCollider2D id, LightingBuffer2D buffer) {
            Vector2 newPosition = Vector2.zero;
            newPosition.x = buffer.lightSource.transform.position.x;
            newPosition.y = buffer.lightSource.transform.position.y;

            Vector3 rot = Math2D.GetPitchYawRollRad(id.transform.rotation);

            float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);
            float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
        

            float sx = 1; 
            sx /= id.properties.cellSize.x;
            sx /= id.transform.lossyScale.x;
            sx /= rotationXScale;


            float sy = 1;
            sy /= id.properties.cellSize.y;
            sy /= id.transform.lossyScale.y;
            sy /= rotationYScale;


            newPosition.x *= sx;
            newPosition.y *= sy;

            Vector2 tilemapPosition = Vector2.zero;

            tilemapPosition.x -= id.properties.area.position.x;
            tilemapPosition.y -= id.properties.area.position.y;
            
            tilemapPosition.x -= id.transform.position.x;
            tilemapPosition.y -= id.transform.position.y;
                
            tilemapPosition.x -= id.properties.cellAnchor.x;
            tilemapPosition.y -= id.properties.cellAnchor.y;

            // Cell Size Is Not Calculated Correctly
            tilemapPosition.x += 1;
            tilemapPosition.y += 1;
            
            newPosition.x += tilemapPosition.x;
            newPosition.y += tilemapPosition.y;

            return(new Vector2Int((int)newPosition.x, (int)newPosition.y));
        }
    }
}