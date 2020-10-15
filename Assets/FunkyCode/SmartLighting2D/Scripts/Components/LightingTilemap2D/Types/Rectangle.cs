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

        public RectangleMap map;

		public List<Polygon2D> compositeColliders = new List<Polygon2D>();

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

			map = new RectangleMap();

			properties.arraySize = new Vector2Int(properties.area.size.x + diffX, properties.area.size.y + diffY);

			map.map = new LightingTile[properties.arraySize.x, properties.arraySize.y];

			for(int sx = 0; sx <= properties.area.size.x; sx++) {
				for(int sy = 0; sy <= properties.area.size.y; sy++) {
					map.map[sx, sy] = null;
				}
			}

			TilemapCollider2D tilemapCollider = gameObject.GetComponent<TilemapCollider2D>();
			if (tilemapCollider != null) {
				properties.colliderOffset = tilemapCollider.offset;
			}

			properties.cellAnchor += properties.colliderOffset;

			compositeColliders.Clear();

			InitializeGrid();
			InitializeCompositeCollider();
        }

		private void InitializeCompositeCollider() {
			CompositeCollider2D compositeCollider2D = gameObject.GetComponent<CompositeCollider2D>();

			if (compositeCollider2D != null) {
				compositeColliders = Polygon2DHelper.CreateFromCompositeCollider(compositeCollider2D);
			}
		}

		private void InitializeGrid() {
			Tilemap tilemap2D = properties.tilemap;

			ITilemap tilemap = GetITilemap(tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {
					tilebase.GetTileData(position, tilemap, ref tileData);
					
					LightingTile lightingTile = new LightingTile();
					lightingTile.SetOriginalSprite(tileData.sprite);
					lightingTile.GetShapePolygons();
					lightingTile.colliderType = tileData.colliderType;
					//tileData.

					int sx = position.x + properties.area.size.x / 2;
					int sy = position.y + properties.area.size.y / 2;

					if (sx < 0 || sy < 0) {
						continue;
					}

					if (sx >= properties.arraySize.x || sy >= properties.arraySize.y) {
						continue;
					}

					map.map[sx, sy] = lightingTile;
				}
			}
		}
        
		public class RectangleMap {
			public List<Tile> mapTiles = new List<Tile>();
			public LightingTile[,] map;

			public int width;
			public int height;
		}

		public class Tile {
			public Vector2Int position;

			public LightingTile tile;
		}

    }
}
