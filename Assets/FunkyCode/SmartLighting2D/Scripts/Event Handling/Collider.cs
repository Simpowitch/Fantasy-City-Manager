using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventHandling {

    public class Collider : Base {

        static public void GetCollisions(List<LightCollision2D> collisions, LightingSource2D lightingSource) {
            List<LightingCollider2D> colliderList = LightingCollider2D.GetList();

            foreach (LightingCollider2D id in colliderList) { // Why all and not selected?
                if (id.mainShape.colliderType == LightingCollider2D.ColliderType.None) {
                    continue;
                }

                if (id.transform == null) {
                    continue;
                }

                if (lightingSource == null) {
                    continue;
                }

                if (Vector2.Distance(id.transform.position, lightingSource.transform.position) > id.mainShape.GetRadiusWorld() + lightingSource.size) {
                    continue;
                }

                List<Polygon2D> localPolys = id.mainShape.GetPolygonsLocal();
                
                if (localPolys.Count < 1) {
                    continue;
                }

                Polygon2 polygon = new Polygon2(localPolys[0]);
                polygon.ToWorldSpaceSelf(id.transform);
                polygon.ToOffsetItself(-lightingSource.transform.position);

                LightCollision2D collision = new LightCollision2D();
                collision.lightSource = lightingSource;
                collision.collider = id;

                if (collision.pointsColliding == null) {
                    collision.pointsColliding = new List<Vector2>();
                }
                
                foreach(Vector2 point in polygon.points) {
                    if (point.magnitude < lightingSource.size) {
                   
                        float direction = point.Atan2(Vector2.zero) * Mathf.Rad2Deg;

                        direction = (direction + 1080 - 90 - lightingSource.transform2D.rotation) % 360;

                        if (direction <= lightingSource.angle / 2|| direction >= 360 - lightingSource.angle / 2) {
                            collision.pointsColliding.Add(point);
                        }
                    }
                }

                collisions.Add(collision);
            }
        }

        static List<Vector2> removePointsColliding = new List<Vector2>();
        static List<LightCollision2D> removeCollisions = new List<LightCollision2D>();
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
        static Pair2D p = new Pair2D(Vector2D.Zero(), Vector2D.Zero());

        public static List<LightCollision2D> RemoveHiddenCollisions(List<LightCollision2D> collisions, LightingSource2D lightingSource) {
            List<LightingCollider2D> colliderList = LightingCollider2D.GetList();
            
            float lightSizeSquared = Mathf.Sqrt(lightingSource.size * lightingSource.size + lightingSource.size * lightingSource.size);
            double rot;	
            
            Polygon2D triPoly = GetPolygon();

            Vector2 scale = Vector2.one;
            Vector2 offset = Vector2.zero;

            foreach (LightingCollider2D id in colliderList) {
                if (Vector2.Distance(id.transform.position, lightingSource.transform.position) > id.mainShape.GetRadiusWorld() + lightingSource.size) {
                    continue;
                }

                if (id.mainShape.colliderType == LightingCollider2D.ColliderType.None) {
                    continue;
                }

                List<Polygon2D> polygons = id.mainShape.GetPolygonsWorld();

                if (polygons.Count < 1) {
                    continue;
                }

                offset.x = - lightingSource.transform.position.x;
                offset.y = - lightingSource.transform.position.y;

                removePointsColliding.Clear();
                removeCollisions.Clear();

                for(int i = 0; i < polygons.Count; i++) {

                    List<Vector2D> pointsList = polygons[i].pointsList;
                    int pointsCount = pointsList.Count;

                    for(int x = 0; x < pointsCount; x++) {
                        p.A.x = pointsList[x].x;
                        p.A.y = pointsList[x].y;

                        p.B.x = pointsList[(x + 1) % pointsCount].x ;
                        p.B.y = pointsList[(x + 1) % pointsCount].y ;

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
            return(collisions);
        }
    }
}