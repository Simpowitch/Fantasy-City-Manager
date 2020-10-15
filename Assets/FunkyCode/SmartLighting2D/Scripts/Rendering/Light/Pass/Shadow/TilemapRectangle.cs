using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapRectangle : Base {

        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.rectangle.colliderType == LightingTilemapCollider.Rectangle.ColliderType.None) {
                return;
            }

            if (id.rectangle.colliderType == LightingTilemapCollider.Rectangle.ColliderType.CompositeCollider) {
                return;
            }

            TilemapProperties properties = id.rectangle.Properties;
            Vector2 offset = -buffer.lightSource.transform.position;
            bool isGrid = id.rectangle.colliderType == LightingTilemapCollider.Rectangle.ColliderType.Grid;

            Vector2 scale = Rectangle.GetScale(id, isGrid);

            int tilemapSize = Rectangle.Light.GetSize(id, buffer);
			Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= properties.arraySize.x || y >= properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangle.map.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    switch(id.colliderTileType) {
                        case LightingTilemapCollider2D.ShadowTileType.AllTiles:
                        break;

                        case LightingTilemapCollider2D.ShadowTileType.ColliderOnly:
                            if (tile.colliderType == UnityEngine.Tilemaps.Tile.ColliderType.None) {
                                continue;
                            }
                        break;
                    }

                    List<Polygon2D> polygons = tile.GetPolygons(id);

                    if (polygons == null || polygons.Count < 1) {
                        continue;
                    }

                    Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

                    tilePosition += offset;
 
                    if (tile.InRange(tilePosition, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    if (x-1 > 0 && y-1 > 0 && x + 1 < properties.area.size.x && y + 1 < properties.area.size.y) {
                        if (tilePosition.x > 0 && tilePosition.y > 0) {
                            LightingTile tileA = id.rectangle.map.map[x - 1, y];
                            LightingTile tileB = id.rectangle.map.map[x, y - 1];
                            LightingTile tileC = id.rectangle.map.map[x - 1, y - 1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (tilePosition.x < 0 && tilePosition.y > 0) {
                            LightingTile tileA = id.rectangle.map.map[x+1, y];
                            LightingTile tileB = id.rectangle.map.map[x, y-1];
                            LightingTile tileC = id.rectangle.map.map[x+1, y-1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (tilePosition.x > 0 && tilePosition.y < 0) {
                            LightingTile tileA = id.rectangle.map.map[x-1, y];
                            LightingTile tileB = id.rectangle.map.map[x, y+1];
                            LightingTile tileC = id.rectangle.map.map[x-1, y+1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (tilePosition.x < 0 && tilePosition.y < 0) {
                            LightingTile tileA = id.rectangle.map.map[x+1, y];
                            LightingTile tileB = id.rectangle.map.map[x, y+1];
                            LightingTile tileC = id.rectangle.map.map[x+1, y+1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        }
                    }

                    Shadow.Main.Draw(buffer, polygons, lightSizeSquared, z, tilePosition, scale, 0);
                }
            }
        }
    }
}