using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapRectangle : Base {

        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.colliderType == LightingTilemapCollider2D.ColliderType.None) {
                return;
            }

            if (id.colliderType == LightingTilemapCollider2D.ColliderType.Collider) {
                return;
            }

            if (id.rectangleMap == null) {
                return;
            }
            
            if (id.rectangleMap.map == null) {
                return;
            }

            Vector2 positionScale = GetPositionScale(id);
            Vector2 tilemapOffset = GetTilemapOffset(id);
            int tilemapSize = GetTilemapSize(id, buffer);
            Vector2 offset = -buffer.lightSource.transform.position;
            Vector2Int tilemapLightPosition = GetTilemapLightPosition(id, buffer);

            Vector2 polyOffset;

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= id.properties.arraySize.x || y >= id.properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangleMap.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    if (tile.colliderType != id.colliderTileType) {
                        continue;
                    }
                
                    polygons = tile.GetPolygons(id);

                    if (polygons == null || polygons.Count < 1) {
                        continue;
                    }

                    polyOffset.x = (x + tilemapOffset.x) * positionScale.x;
                    polyOffset.y = (y + tilemapOffset.y) * positionScale.y;
            
                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    if (x-1 > 0 && y-1 > 0 && x + 1 < id.properties.area.size.x && y + 1 < id.properties.area.size.y) {
                        if (polyOffset.x > buffer.lightSource.transform.position.x && polyOffset.y > buffer.lightSource.transform.position.y) {
                            LightingTile tileA = id.rectangleMap.map[x-1, y];
                            LightingTile tileB = id.rectangleMap.map[x, y-1];
                            LightingTile tileC = id.rectangleMap.map[x-1, y-1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (polyOffset.x < buffer.lightSource.transform.position.x && polyOffset.y > buffer.lightSource.transform.position.y) {
                            LightingTile tileA = id.rectangleMap.map[x+1, y];
                            LightingTile tileB = id.rectangleMap.map[x, y-1];
                            LightingTile tileC = id.rectangleMap.map[x+1, y-1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (polyOffset.x > buffer.lightSource.transform.position.x && polyOffset.y < buffer.lightSource.transform.position.y) {
                            LightingTile tileA = id.rectangleMap.map[x-1, y];
                            LightingTile tileB = id.rectangleMap.map[x, y+1];
                            LightingTile tileC = id.rectangleMap.map[x-1, y+1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        } else if (polyOffset.x < buffer.lightSource.transform.position.x && polyOffset.y < buffer.lightSource.transform.position.y) {
                            LightingTile tileA = id.rectangleMap.map[x+1, y];
                            LightingTile tileB = id.rectangleMap.map[x, y+1];
                            LightingTile tileC = id.rectangleMap.map[x+1, y+1];
                            if (tileA != null && tileB != null && tileC != null) {
                                continue;
                            }
                        }
                    }

                    polyOffset += offset;

                    Rendering.Light.Shadow.Polygon.Draw(buffer, polygons, lightSizeSquared, z, polyOffset, id.transform.lossyScale);
                }
            }
        }
    }
}