using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using LightingSettings;

#if UNITY_2017_4_OR_NEWER

	using UnityEngine.Tilemaps;

	[ExecuteInEditMode]
	public class LightingTilemapCollider2D : MonoBehaviour {
		public enum MapType {UnityEngineTilemapRectangle, UnityEngineTilemapIsometric, UnityEngineTilemapHexagon, SuperTilemapEditor};
		
		public enum ColliderType {None, Grid, SpriteCustomPhysicsShape, Collider};
		public enum MaskType {None, Grid, Sprite, BumpedSprite, SpriteCustomPhysicsShape};

		public MapType mapType = MapType.UnityEngineTilemapRectangle;

		public LightingLayer lightingCollisionLayer = LightingLayer.Layer1;
		public LightingLayer lightingMaskLayer = LightingLayer.Layer1;
		
		public ColliderType colliderType = ColliderType.Grid;
		public MaskType maskType = MaskType.Sprite;

		public Tile.ColliderType colliderTileType = Tile.ColliderType.None;

		public NormalMapMode bumpMapMode = new NormalMapMode();

		// Should be improved
		public List<Polygon2D> edgeColliders = new List<Polygon2D>();
		public List<Polygon2D> polygonColliders = new List<Polygon2D>();

		public TilemapProperties properties = new TilemapProperties();

		private Tilemap tilemap2D;

		public IsometricMap isometricMap;
		public RectangleMap rectangleMap;

		public SuperTilemapEditorSupport.TilemapCollider2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapCollider2D();

		public static List<LightingTilemapCollider2D> list = new List<LightingTilemapCollider2D>();

		public void OnEnable() {
			list.Add(this);

			LightingManager2D.Get();

			Initialize();

			LightingSource2D.ForceUpdateAll();
		}

		public void OnDisable() {
			list.Remove(this);

			LightingSource2D.ForceUpdateAll();
		}

		static public List<LightingTilemapCollider2D> GetList() {
			return(list);
		}

		public void Initialize() {
			switch(mapType) {
				case MapType.UnityEngineTilemapRectangle:
					InitializeUnityRectangle();

				break;

				case MapType.UnityEngineTilemapIsometric:
					InitializeUnityIsometric();

				break;

				case MapType.SuperTilemapEditor:
					InitializeSuperTilemapEditor();

				break;
			}
		}

		void InitializeUnityRectangle() {
			tilemap2D = GetComponent<Tilemap>();

			if (tilemap2D == null) {
				return;
			}

			Grid grid = tilemap2D.layoutGrid;

			if (grid == null) {
				Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid", gameObject);
			} else {
				properties.cellSize = grid.cellSize;
				properties.cellGap = grid.cellGap;
			}

			properties.cellAnchor = tilemap2D.tileAnchor;

			int maxSizePosX = tilemap2D.cellBounds.size.x + Mathf.Abs(tilemap2D.cellBounds.xMin);
			int maxSizePosY = tilemap2D.cellBounds.size.y + Mathf.Abs(tilemap2D.cellBounds.yMin);

			//Debug.Log(tilemap2D.cellBounds);

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

			rectangleMap = new RectangleMap();

			properties.arraySize = new Vector2Int(properties.area.size.x + diffX, properties.area.size.y + diffY);

			//Debug.Log(properties.area.size.x + diffX + " " + properties.area.size.y + diffY);

			rectangleMap.map = new LightingTile[properties.arraySize.x, properties.arraySize.y];

			for(int sx = 0; sx <= properties.area.size.x; sx++) {
				for(int sy = 0; sy <= properties.area.size.y; sy++) {
					rectangleMap.map[sx, sy] = null;
				}
			}

			TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();
			if (tilemapCollider != null) {
				properties.colliderOffset = tilemapCollider.offset;
			}

			properties.cellAnchor += properties.colliderOffset;

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

					int sx = position.x + properties.area.size.x / 2;
					int sy = position.y + properties.area.size.y / 2;

					if (sx < 0 || sy < 0) {
						continue;
					}

					if (sx >= properties.arraySize.x || sy >= properties.arraySize.y) {
						continue;
					}

					rectangleMap.map[sx, sy] = lightingTile;
				}
			}
			edgeColliders.Clear();
			polygonColliders.Clear();
		}

		void InitializeUnityIsometric() {
			tilemap2D = GetComponent<Tilemap>();

			if (tilemap2D == null) {
				return;
			}

			Grid grid = tilemap2D.layoutGrid;

			if (grid == null) {
				Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid", gameObject);
			} else {
				properties.cellSize = grid.cellSize;
				properties.cellGap = grid.cellGap;
			}

			properties.cellAnchor = tilemap2D.tileAnchor;

			isometricMap = new IsometricMap();

			ITilemap tilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tilemap, tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {
					tilebase.GetTileData(position, tilemap, ref tileData);

					//if (onlyColliders && tileData.colliderType == Tile.ColliderType.None) {
					//	continue;
					//}

					IsometricTile tile = new IsometricTile();
					tile.position = position;
					
					LightingTile lightingTile = new LightingTile();
					lightingTile.SetOriginalSprite(tileData.sprite);
					lightingTile.GetShapePolygons();

					tile.tile = lightingTile;

					isometricMap.mapTiles.Add(tile);

				}
			}

			edgeColliders.Clear();
			polygonColliders.Clear();
		}

		public static ITilemap GetITilemap(Tilemap tilemap) {
			ITilemap iTilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(iTilemap, tilemap);
			return iTilemap;
		}

		void InitializeSuperTilemapEditor() {
			superTilemapEditor.tilemapCollider2D = this;

			superTilemapEditor.Initialize();
		}

		public class TilemapProperties {
			public Vector2 cellSize = new Vector2(1, 1);
			public Vector2 cellAnchor = new Vector2(0.5f, 0.5f);
			public Vector2 cellGap = new Vector2(1, 1);
			public Vector2 colliderOffset = new Vector2(0, 0);
			public BoundsInt area;

			public Vector2Int arraySize = new Vector2Int();
		}

		public class IsometricMap {
			public List<IsometricTile> mapTiles = new List<IsometricTile>();
			//public LightingTile[,] map;
		}

		public class IsometricTile {
			public Vector3Int position;

			public LightingTile tile;
		}

		public class RectangleMap {
			public List<RectangleTile> mapTiles = new List<RectangleTile>();
			public LightingTile[,] map;

			public int width;
			public int height;
		}

		public class RectangleTile {
			public Vector2Int position;

			public LightingTile tile;
		}

	}

#endif