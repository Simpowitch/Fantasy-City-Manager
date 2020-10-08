using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day.WithoutAtlas {

    public class Shadow {
        const float uv0 = 0;
        const float uv1 = 1;
        const float pi2 = Mathf.PI / 2;
        
        static Vector2 zA = Vector2.zero, zB = Vector2.zero, zC = Vector2.zero;
        static Vector2 pA =  Vector2.zero, pB = Vector2.zero;
        static Vector2 objectOffset = Vector2.zero;

        static DayLightingCollider2D id;
        static List<DayLightingCollider2D> colliderList;

        static List<DoublePair2D> polygonPairs;
        static DoublePair2D p;

        static public void Draw(Camera camera, Vector2 position, float z, int layer) {
            colliderList = DayLightingCollider2D.GetList();
            int colliderCount = colliderList.Count;

            Lighting2D.materials.GetShadowBlur().SetPass (0);
            
            GL.Begin(GL.TRIANGLES);

            float intensity = Lighting2D.dayLightingSettings.softness.intensity;
            bool softness = Lighting2D.dayLightingSettings.softness.enable;
            
            for(int i = 0; i < colliderCount; i++) {
                id = colliderList[i];
                
                if ((int)id.collisionDayLayer != layer) {
                    continue;
                }

                if (id.shape.colliderType == DayLightingCollider2D.ColliderType.None) {
                    continue;
                }

                if (id.shape.colliderType == DayLightingCollider2D.ColliderType.Sprite) {
                    continue;
                }

                if (id.shape.height < 0) {
                    continue;
                }

                DayLighting.ShadowMesh shadow = id.shadowMesh;
                if (shadow == null) {
                    continue;
                }

                if (id.InAnyCamera() == false) {
					continue;
				}
                

                Vector3 pos = new Vector3(id.transform2D.position.x + position.x, id.transform2D.position.y + position.y, z);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, 0), Vector3.one);
            
                GL.Color(Color.black);

                foreach(Mesh mesh in shadow.softMeshes) {
                    //Graphics.DrawMeshNow(mesh, matrix);
                    GLExtended.DrawMeshPass(new MeshObject(mesh), pos, Vector3.one, 0);
                }

                foreach(Mesh mesh in shadow.meshes) {
                    //Graphics.DrawMeshNow(mesh, matrix);
                     GLExtended.DrawMeshPass(new MeshObject(mesh), pos, Vector3.one, 0);
                }    
            }

            GL.End();
        }
    }
}