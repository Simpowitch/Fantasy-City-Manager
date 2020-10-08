using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTilemapEditorSupport {

    [System.Serializable]
    public class TilemapCollider2D {
        
		public enum ColliderType {None, Grid, Collider};
		public enum MaskType {None, Grid, Sprite};

		public ColliderType colliderType = ColliderType.Grid;
		public MaskType maskType = MaskType.Sprite;

        public List<Polygon2D> edgeColliders = new List<Polygon2D>();
		public List<Polygon2D> polygonColliders = new List<Polygon2D>();

        public LightingTilemapCollider2D tilemapCollider2D;

        #if (SUPER_TILEMAP_EDITOR)
            public CreativeSpore.SuperTilemapEditor.STETilemap tilemapSTE;
            public STEMap SuperTilemapEditorMap;
        #endif
    
        #if (SUPER_TILEMAP_EDITOR)
            public void Initialize() {
                tilemapSTE = tilemapCollider2D.GetComponent<CreativeSpore.SuperTilemapEditor.STETilemap>();

                if (tilemapSTE == null) {
                    return;
                }

                SuperTilemapEditorMap = new STEMap();
                SuperTilemapEditorMap.width = tilemapSTE.GridWidth;
                SuperTilemapEditorMap.height = tilemapSTE.GridHeight;
            
                tilemapCollider2D.properties.cellSize = tilemapSTE.CellSize;

                SuperTilemapEditorMap.map = new LightingTile[tilemapSTE.GridWidth + 2, tilemapSTE.GridHeight + 2];

                tilemapCollider2D.properties.area.position = new Vector3Int((int)tilemapSTE.MapBounds.center.x, (int)tilemapSTE.MapBounds.center.y, 0);

                tilemapCollider2D.properties.area.size = new Vector3Int((int)(tilemapSTE.MapBounds.extents.x + 1) * 2, (int)(tilemapSTE.MapBounds.extents.y + 1) * 2, 0);

                for(int x = 0; x <= tilemapSTE.GridWidth; x++) {
                    for(int y = 0; y <= tilemapSTE.GridHeight; y++) {
                        SuperTilemapEditorMap.map[x, y] = null;
                    }
                }

                for(int x = 0; x <= tilemapSTE.GridWidth; x++) {
                    for(int y = 0; y <= tilemapSTE.GridHeight; y++) {
                        int tileX = x + tilemapCollider2D.properties.area.position.x - tilemapCollider2D.properties.area.size.x / 2;
                        int tileY = y + tilemapCollider2D.properties.area.position.y - tilemapCollider2D.properties.area.size.y / 2;

                        CreativeSpore.SuperTilemapEditor.Tile tileSTE = tilemapSTE.GetTile(tileX, tileY);

                        if (tileSTE == null) {
                            continue;
                        }

                        LightingTile lightingTile = new LightingTile();
                        SuperTilemapEditorMap.map[x, y] = lightingTile;

                        STETile tile = new STETile();
                        tile.position = new Vector2Int(x, y);

                        tile.tile = lightingTile;
                        tile.uv = tileSTE.uv;

                        SuperTilemapEditorMap.mapTiles.Add(tile);
                    }
                }	

                edgeColliders.Clear();
                polygonColliders.Clear();

                if (colliderType == ColliderType.Collider) {
                    foreach(Transform t in tilemapCollider2D.transform) {
                        foreach(Component c in t.GetComponents<EdgeCollider2D>()) {
                            Polygon2D poly = Polygon2DHelper.CreateFromEdgeCollider(c as EdgeCollider2D);
                            poly = poly.ToWorldSpace(t);
                            edgeColliders.Add(poly);
                        }
                        foreach(Component c in t.GetComponents<PolygonCollider2D>()) {
                            Polygon2D poly = Polygon2DList.CreateFromPolygonColliderToWorldSpace(c as PolygonCollider2D)[0];
                            polygonColliders.Add(poly);
                        }
                    }			
                }
            }
        #else
            public void Initialize() {}

        #endif

        public class STEMap {
			public List<STETile> mapTiles = new List<STETile>();
			public LightingTile[,] map;

			public int width;
			public int height;
		}

		public class STETile {
			public Vector2Int position;

			public LightingTile tile;

			public Rect uv;
		}

    }
}