using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventHandling {
    public class Tilemap2D : Base {

        static List<Vector2> removePointsColliding = new List<Vector2>();
        static List<LightCollision2D> removeCollisions = new List<LightCollision2D>();
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
        static Pair2D p = new Pair2D(Vector2D.Zero(), Vector2D.Zero());
    
        public static List<LightCollision2D> RemoveHiddenCollisions(List<LightCollision2D> collisions, LightingSource2D lightingSource) {
            float lightSizeSquared = Mathf.Sqrt(lightingSource.size * lightingSource.size + lightingSource.size * lightingSource.size);
            double rot;	
            
            Polygon2D triPoly = GetPolygon();
            Vector2 offset = -lightingSource.transform.position;

            foreach(LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
                if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                    continue;
                }

                if (id.rectangle.colliderType != LightingTilemapCollider.Rectangle.ColliderType.Grid) {
                    continue;
                }

                LightingBuffer2D buffer2D = lightingSource.GetBuffer();

                Vector2 scale = Vector2.one;

                foreach(LightingTile tile in id.rectangle.mapTiles) {
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

                        Vector2 tilePosition = Rendering.Light.Rectangle.GetTilePosition(tile.position.x, tile.position.y, id);

                        tilePosition += offset;
    
                        if (tile.InRange(tilePosition, 2 + buffer2D.lightSource.size)) {
                            continue;
                        }

                        removePointsColliding.Clear();
                        removeCollisions.Clear();

                        for(int i = 0; i < polygons.Count; i++) {

                            List<Vector2D> pointsList = polygons[i].pointsList;
                            int pointsCount = pointsList.Count;

                            for(int z = 0; z < pointsCount; z++) {
                                p.A.x = pointsList[z].x;
                                p.A.y = pointsList[z].y;

                                p.B.x = pointsList[(z + 1) % pointsCount].x ;
                                p.B.y = pointsList[(z + 1) % pointsCount].y ;

                                vA.x = p.A.x * scale.x + offset.x;
                                vA.y = p.A.y * scale.y + offset.y;

                                vB.x = p.B.x * scale.x + offset.x;
                                vB.y = p.B.y * scale.y + offset.y;

                                vC.x = p.A.x * scale.x + offset.x;
                                vC.y = p.A.y * scale.y + offset.y;

                                vD.x = p.B.x * scale.x + offset.x;
                                vD.y = p.B.y * scale.y + offset.y;
                                
                                rot = System.Math.Atan2 (vA.y, vA.x);
                                vA.x += System.Math.Cos(rot) * lightSizeSquared;
                                vA.y += System.Math.Sin(rot) * lightSizeSquared;

                                rot = System.Math.Atan2 (vB.y, vB.x);
                                vB.x += System.Math.Cos(rot) * lightSizeSquared;
                                vB.y += System.Math.Sin(rot) * lightSizeSquared;

                                triPoly.pointsList[0].x = vA.x;
                                triPoly.pointsList[0].y = vA.y;

                                triPoly.pointsList[1].x = vB.x;
                                triPoly.pointsList[1].y = vB.y;

                                triPoly.pointsList[2].x = vD.x;
                                triPoly.pointsList[2].y = vD.y;

                                triPoly.pointsList[3].x = vC.x;
                                triPoly.pointsList[3].y = vC.y;

                                foreach(LightCollision2D col in collisions) {
                                    if (col.collider == id) {
                                        continue;
                                    }

                                    foreach(Vector2 point in col.pointsColliding) {
                                        if (triPoly.PointInPoly(point)) {
                                            removePointsColliding.Add(point);
                                        }
                                    }

                                    foreach(Vector2 point in removePointsColliding) {
                                        col.pointsColliding.Remove(point);
                                    }

                                    removePointsColliding.Clear();
                                    
                                    if (col.pointsColliding.Count < 1) {
                                        removeCollisions.Add(col);
                                    }
                                }

                                foreach(LightCollision2D col in removeCollisions) {
                                    collisions.Remove(col);
                                }

                                removeCollisions.Clear();
                            }
                        }

                    }
                       
            }
            
            return(collisions);
        }
    }
}


