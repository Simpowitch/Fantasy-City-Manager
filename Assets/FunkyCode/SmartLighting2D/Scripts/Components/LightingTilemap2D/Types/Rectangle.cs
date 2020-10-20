using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LightingTilemapCollider {

	[System.Serializable]
    public class Rectangle : Base {
		public enum ColliderType {None, Grid, SpriteCustomPhysicsShape, CompositeCollider};
		public enum MaskType {None, Grid, Sprite, BumpedSprite, SpriteCustomPhysicsShape};
		
		public ColliderType colliderType = ColliderType.Grid;
		public MaskType maskType = MaskType.Sprite;

		public List<Polygon2D> compositeColliders = new List<Polygon2D>();

		public List<LightingTile> mapTiles = new List<LightingTile>();

        public static ITilemap GetITilemap(Tilemap tilemap) {
			ITilemap iTilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(iTilemap, tilemap);
			return iTilemap;
		}

        public void Initialize() {
			if (UpdateProperties() == false) {
				return;
			}
						
			Tilemap tilemap2D = properties.tilemap;

			int maxSizePosX = tilemap2D.cellBounds.size.x + Mathf.Abs(tilemap2D.cellBounds.xMin);
			int maxSizePosY = tilemap2D.cellBounds.size.y + Mathf.Abs(tilemap2D.cellBounds.yMin);

			int diffY = tilemap2D.cellBounds.yMin + tilemap2D.cellBounds.size.y - 1;
			if (diffY > 0) {
				maxSizePosY -= diffY;
			} else {
				diffY = 1;
			}

			int diffX = tilemap2D.cellBounds.xMin + tilemap2D.cellBounds.size.x - 1;
			if (diffX > 0) {
				maxSizePosX -= diffX;
			} else {
				diffX = 1;
			}

			properties.area = new BoundsInt(tilemap2D.cellBounds.xMin, tilemap2D.cellBounds.yMin, 0, maxSizePosX, maxSizePosY, 1);

			TileBase[] tileArray = tilemap2D.GetTilesBlock(properties.area);

			properties.arraySize = new Vector2Int(properties.area.size.x + diffX, properties.area.size.y + diffY);
			
			TilemapCollider2D tilemapCollider = gameObject.GetComponent<TilemapCollider2D>();
			if (tilemapCollider != null) {
				properties.colliderOffset = tilemapCollider.offset;
			}

			properties.cellAnchor += properties.colliderOffset;

			InitializeGrid();
			InitializeCompositeCollider();
        }

		private void InitializeCompositeCollider() {
			compositeColliders.Clear();
			
			CompositeCollider2D compositeCollider2D = gameObject.GetComponent<CompositeCollider2D>();

			if (compositeCollider2D != null) {
				compositeColliders = Polygon2DHelper.CreateFromCompositeCollider(compositeCollider2D);
			}
		}

		private void InitializeGrid() {
			mapTiles.Clear();

			Tilemap tilemap2D = properties.tilemap;

			ITilemap tilemap = GetITilemap(tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {
					tilebase.GetTileData(position, tilemap, ref tileData);
					
					LightingTile lightingTile = new LightingTile();
				
					int sx = position.x + properties.area.size.x / 2;
					int sy = position.y + properties.area.size.y / 2;

					lightingTile.position = new Vector3Int(sx, sy,0);

					lightingTile.SetOriginalSprite(tileData.sprite);
					lightingTile.GetShapePolygons();

					lightingTile.colliderType = tileData.colliderType;

					mapTiles.Add(lightingTile);
				}
			}
		}
    }
}