using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Legacy {

        public static Pair2D pair = Pair2D.Zero();

        public static void Draw(LightingBuffer2D buffer, List<Polygon2D> polygons, Vector2 scale, float height) {
            Vector2 offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            float lightSizeSquared = ShadowEngine.lightSize;
            float z = ShadowEngine.shadowZ;

            float outerAngle = buffer.lightSource.outerAngle;
            bool drawInside = false;
            bool culling = true;

            Rect penumbraRect = ShadowEngine.Penumbra.uvRect;
			
            Vector2 projectedMiddle, projectedLeft, projectedRight, outerLeft, outerRight;
            Vector2 edgeAWorld;
            Vector2 edgeBWorld;

            float angleA, angleB;
            float rotA, rotB, rot;

            int PolygonCount = polygons.Count;

            if (height > 0) {
                lightSizeSquared = height;
                outerAngle = 0;
                culling = false;
            }

            for(int i = 0; i < PolygonCount; i++) {

                if (ShadowEngine.lightDrawAbove == false && polygons[i].PointInPoly (-offset)) {
                    drawInside = true;
                } else {
                    drawInside = false;
                }

                List<Vector2D> pointsList = polygons[i].pointsList;
                int pointsCount = pointsList.Count;
            
                for(int x = 0; x < pointsCount; x++) {
                    int next = (x + 1) % pointsCount;
                    
                    pair.A = pointsList[x];
                    pair.B = pointsList[next];

                    float edgeALocalX = (float)pair.A.x * scale.x;
                    float edgeALocalY = (float)pair.A.y * scale.y;

                    float edgeBLocalX = (float)pair.B.x * scale.x;
                    float edgeBLocalY = (float)pair.B.y * scale.y;

                    edgeAWorld.x = edgeALocalX + offset.x;
                    edgeAWorld.y = edgeALocalY + offset.y;

                    edgeBWorld.x = edgeBLocalX + offset.x;
                    edgeBWorld.y = edgeBLocalY + offset.y;

                    float lightDirection = Mathf.Atan2((edgeAWorld.y + edgeBWorld.y) / 2 , (edgeAWorld.x + edgeBWorld.x) / 2 ) * Mathf.Rad2Deg;
                    float EdgeDirection = Mathf.Atan2(edgeALocalY - edgeBLocalY, edgeALocalX - edgeBLocalX) * Mathf.Rad2Deg - 180;

                    lightDirection -= EdgeDirection;
                    lightDirection = (lightDirection + 720) % 360;
                    
                    if (culling) {
                        if (drawInside) {
                            if (lightDirection > 180) {
                                continue;
                            }
                        } else {
                            if (lightDirection < 180) {
                                continue;
                            }
                        }
                    }

                    angleA = (float)System.Math.Atan2 (edgeAWorld.y, edgeAWorld.x);
                    angleB = (float)System.Math.Atan2 (edgeBWorld.y, edgeBWorld.x);

                    rotA = angleA - Mathf.Deg2Rad * buffer.lightSource.outerAngle;
                    rotB = angleB + Mathf.Deg2Rad * buffer.lightSource.outerAngle;

                    projectedRight.x = edgeAWorld.x + Mathf.Cos(angleA) * lightSizeSquared;
                    projectedRight.y = edgeAWorld.y + Mathf.Sin(angleA) * lightSizeSquared;

                    projectedLeft.x = edgeBWorld.x + Mathf.Cos(angleB) * lightSizeSquared;
                    projectedLeft.y = edgeBWorld.y + Mathf.Sin(angleB) * lightSizeSquared;

                    outerRight.x = edgeAWorld.x + Mathf.Cos(rotA) * lightSizeSquared;
                    outerRight.y = edgeAWorld.y + Mathf.Sin(rotA) * lightSizeSquared;
                    
                    outerLeft.x = edgeBWorld.x + Mathf.Cos(rotB) * lightSizeSquared;
                    outerLeft.y = edgeBWorld.y + Mathf.Sin(rotB) * lightSizeSquared;

                    if (outerAngle > 0) {
                        GL.Color(Color.white);

                        // Right Penumbra
                        GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
                        GL.Vertex3(edgeAWorld.x, edgeAWorld.y, z);

                        GL.TexCoord3(penumbraRect.width, penumbraRect.y, 0);
                        GL.Vertex3(outerRight.x, outerRight.y, z);
                        
                        GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
                        GL.Vertex3(projectedRight.x, projectedRight.y, z);
                        
                        // Left Penumbra
                        GL.TexCoord3(penumbraRect.x, penumbraRect.y, 0);
                        GL.Vertex3(edgeBWorld.x, edgeBWorld.y, z);

                        GL.TexCoord3(penumbraRect.width, penumbraRect.y, 0);
                        GL.Vertex3(outerLeft.x, outerLeft.y, z);
                        
                        GL.TexCoord3(penumbraRect.x, penumbraRect.height, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);
                    }

                    GL.Color(Color.black);
              
                    if (ShadowEngine.shadowIterations > 1) {
                        // Right Fin
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedRight.x, projectedRight.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeAWorld.x, edgeAWorld.y, z);

                        // Left Fin
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeAWorld.x, edgeAWorld.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeBWorld.x, edgeBWorld.y, z);
                        
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);

                        Project(projectedLeft, projectedRight, edgeAWorld, edgeBWorld, lightSizeSquared);
                    } else {
                       // Right Fin
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedRight.x, projectedRight.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeAWorld.x, edgeAWorld.y, z);
                        // Left Fin
                        
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeAWorld.x, edgeAWorld.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(edgeBWorld.x, edgeBWorld.y, z);
                        
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);

                        Vector2 p = Math2D.ClosestPointOnLine(Vector2.zero, projectedLeft, projectedRight);
                        rot = (float)System.Math.Atan2 (p.y, p.x);

                        // Detailed Shadow
                        projectedMiddle.x = (edgeAWorld.x + edgeBWorld.x) / 2 + Mathf.Cos(rot) * lightSizeSquared;
                        projectedMiddle.y = (edgeAWorld.y + edgeBWorld.y) / 2 + Mathf.Sin(rot) * lightSizeSquared;                        
                                    
                        // Middle Fin
                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedLeft.x, projectedLeft.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedRight.x, projectedRight.y, z);

                        GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, z);   
                    }
                }
            }
        }

        static void Project(Vector2 projectedLeft, Vector2 projectedRight, Vector2 edgeAWorld, Vector2 edgeBWorld, float lightSize, int iteration = 0) {  
            Vector2 p = Math2D.ClosestPointOnLine(Vector2.zero, projectedLeft, projectedRight);
            Vector2 projectedMiddle;
            float rot;

            // lightSize *= 1.5f;

            projectedMiddle.x = (projectedLeft.x + projectedRight.x) / 2;
            projectedMiddle.y = (projectedLeft.y + projectedRight.y) / 2;
            
            rot = (float)System.Math.Atan2 (projectedMiddle.y, projectedMiddle.x);
            rot = (float)System.Math.Atan2 (p.y, p.x);

            projectedMiddle.x = (edgeAWorld.x + edgeBWorld.x) / 2;
            projectedMiddle.y = (edgeAWorld.y + edgeBWorld.y) / 2;
            projectedMiddle.x += Mathf.Cos(rot) * lightSize;
            projectedMiddle.y += Mathf.Sin(rot) * lightSize;
                        
            // Middle Fin
            GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
            GL.Vertex3(projectedLeft.x, projectedLeft.y, ShadowEngine.shadowZ);

            GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
            GL.Vertex3(projectedRight.x, projectedRight.y, ShadowEngine.shadowZ);

            GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
            GL.Vertex3(projectedMiddle.x, projectedMiddle.y, ShadowEngine.shadowZ);

            iteration += 1;

            if (iteration >= ShadowEngine.shadowIterations) {
                return;
            }

            Project(projectedLeft, projectedMiddle, edgeAWorld, edgeBWorld, lightSize, iteration);
            Project(projectedRight, projectedMiddle, edgeAWorld, edgeBWorld, lightSize, iteration);
        }
    }
}