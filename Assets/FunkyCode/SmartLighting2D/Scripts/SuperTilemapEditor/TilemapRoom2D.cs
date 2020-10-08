using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTilemapEditorSupport {

    public class TilemapRoom2D {

        public LightingTilemapRoom2D lightingTilemapRoom2D;

        #if (SUPER_TILEMAP_EDITOR)
            public CreativeSpore.SuperTilemapEditor.STETilemap tilemapSTE;
            public STEMap SuperTilemapEditorMap;

            private Mesh STEMesh = null;
        #endif

        #if (SUPER_TILEMAP_EDITOR)

            public Mesh GetSTEMesh() {
                if (STEMesh == null) {
                    STEMesh = new Mesh();
                    List<Vector3> vertices = new List<Vector3>();
                    List<Vector2> uv = new List<Vector2>();
                    List<int> triangles = new List<int>();

                    #if (SUPER_TILEMAP_EDITOR)

                    Vector2 uv0, uv1, uv2, uv3;
                    int count = 0;

                    foreach(TilemapRoom2D.STETile STETile in SuperTilemapEditorMap.mapTiles) {
                        Rect rect = STETile.uv;
                        
                        uv0.x = rect.x;
                        uv0.y = rect.y;

                        uv1.x = rect.x + rect.width;
                        uv1.y = rect.y;

                        uv2.x = rect.x + rect.width;
                        uv2.y = rect.y + rect.height;

                        uv3.x = rect.x;
                        uv3.y = rect.y + rect.height;

                        uv.Add(uv0);
                        uv.Add(uv1);
                        uv.Add(uv2);
                        uv.Add(uv3);

                        vertices.Add(new Vector2(STETile.position.x, STETile.position.y));
                        vertices.Add(new Vector2(STETile.position.x + 1, STETile.position.y));
                        vertices.Add(new Vector2(STETile.position.x + 1, STETile.position.y + 1));
                        vertices.Add(new Vector2(STETile.position.x, STETile.position.y + 1));

                        triangles.Add(count + 0);
                        triangles.Add(count + 1);
                        triangles.Add(count + 2);

                        triangles.Add(count + 2);
                        triangles.Add(count + 3);
                        triangles.Add(count + 0);

                        count += 4;
                    }

                    #endif

                    STEMesh.vertices = vertices.ToArray();
                    STEMesh.uv = uv.ToArray();
                    STEMesh.triangles = triangles.ToArray();
                }

                return(STEMesh);
            }

            public void Initialize() {
                tilemapSTE = lightingTilemapRoom2D.GetComponent<CreativeSpore.SuperTilemapEditor.STETilemap>();

                SuperTilemapEditorMap = new STEMap();
                SuperTilemapEditorMap.width = tilemapSTE.GridWidth;
                SuperTilemapEditorMap.height = tilemapSTE.GridHeight;
            
                lightingTilemapRoom2D.properties.cellSize = tilemapSTE.CellSize;

                SuperTilemapEditorMap.map = new LightingTile[tilemapSTE.GridWidth + 2, tilemapSTE.GridHeight + 2];

                lightingTilemapRoom2D.properties.area.position = new Vector3Int((int)tilemapSTE.MapBounds.center.x, (int)tilemapSTE.MapBounds.center.y, 0);

                lightingTilemapRoom2D.properties.area.size = new Vector3Int((int)(tilemapSTE.MapBounds.extents.x + 1) * 2, (int)(tilemapSTE.MapBounds.extents.y + 1) * 2, 0);

                for(int x = 0; x <= tilemapSTE.GridWidth; x++) {
                    for(int y = 0; y <= tilemapSTE.GridHeight; y++) {
                        SuperTilemapEditorMap.map[x, y] = null;
                    }
                }

            //	Debug.Log(properties.area);
            //	Debug.Log(tilemapSTE.MapBounds);

                for(int x = 0; x <= tilemapSTE.GridWidth; x++) {
                    for(int y = 0; y <= tilemapSTE.GridHeight; y++) {
                        int tileX = x + lightingTilemapRoom2D.properties.area.position.x - lightingTilemapRoom2D.properties.area.size.x / 2;
                        int tileY = y + lightingTilemapRoom2D.properties.area.position.y - lightingTilemapRoom2D.properties.area.size.y / 2;

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
                        tile.uv.x += 0.001f;
                        tile.uv.y += 0.001f;
                        tile.uv.width -= 0.001f;
                        tile.uv.height -= 0.001f;

                        SuperTilemapEditorMap.mapTiles.Add(tile);
                    }
                }	

                lightingTilemapRoom2D.edgeColliders.Clear();
                lightingTilemapRoom2D.polygonColliders.Clear();

                /*
                if (lightingTilemapRoom2D.maskType == LightingTilemapRoom2D.MaskType.Collider) {
                    foreach(Transform t in lightingTilemapRoom2D.transform) {
                        foreach(Component c in t.GetComponents<EdgeCollider2D>()) {
                            Polygon2D poly = Polygon2D.CreateFromEdgeCollider(c as EdgeCollider2D);
                            poly = poly.ToWorldSpace(t);
                            lightingTilemapRoom2D.edgeColliders.Add(poly);
                        }
                        foreach(Component c in t.GetComponents<PolygonCollider2D>()) {
                            Polygon2D poly = Polygon2DList.CreateFromPolygonColliderToWorldSpace(c as PolygonCollider2D)[0];
                            lightingTilemapRoom2D.polygonColliders.Add(poly);
                        }
                    }			
                }*/

                lightingTilemapRoom2D.polygonCollidersMeshes = new Mesh[lightingTilemapRoom2D.polygonColliders.Count];
                lightingTilemapRoom2D.edgeCollidersMeshes = new Mesh[lightingTilemapRoom2D.edgeColliders.Count];
            }

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
        #else
            public void Initialize() {}

        #endif
    }
}