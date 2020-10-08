using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public class Bounds {

        private static MeshObject meshObject = null;

        static Vector2 angle0 = Vector2.zero; 
        static Vector2 angle90 = Vector2.zero; 
        static Vector2 angle180 = Vector2.zero; 
        static Vector2 angle270 = Vector2.zero; 

        static Vector2 angle45 = Vector2.zero; 
        static Vector2 angle135 = Vector2.zero; 
        static Vector2 angle225 = Vector2.zero; 
        static Vector2 angle315 = Vector2.zero; 

        public static Vector2 up0 = Vector2.zero; 
        public static Vector2 up1 = Vector2.zero; 
        public static Vector2 up2 = Vector2.zero; 
        public static Vector2 up3 = Vector2.zero; 

        public static Vector2 down0 = Vector2.zero; 
        public static Vector2 down1 = Vector2.zero; 
        public static Vector2 down2 = Vector2.zero; 
        public static Vector2 down3 = Vector2.zero; 

        public static Vector2 left0 = Vector2.zero; 
        public static Vector2 left1 = Vector2.zero; 
        public static Vector2 left2 = Vector2.zero; 
        public static Vector2 left3 = Vector2.zero; 

        public static Vector2 right0 = Vector2.zero; 
        public static Vector2 right1 = Vector2.zero; 
        public static Vector2 right2 = Vector2.zero; 
        public static Vector2 right3 = Vector2.zero; 

        public static void Draw(LightingBuffer2D buffer, Material material, float z) {
            float rotation = buffer.lightSource.transform.rotation.eulerAngles.z + (Mathf.PI / 4) * Mathf.Rad2Deg;
            float size = buffer.lightSource.size;
            size = Mathf.Sqrt(((size * size) + (size * size)));

            Vector3 matrixPosition = new Vector3(0, 0, z);
            Quaternion matrixRotation = Quaternion.Euler(0, 0, rotation);
            Vector3 matrixScale = new Vector3(size, size, 1);
      
            //Graphics.DrawMeshNow(GetMesh(), Matrix4x4.TRS(matrixPosition, matrixRotation, matrixScale));
            
            GLExtended.DrawMesh(GetMesh(), matrixPosition, matrixScale, rotation);

            /*
            GL.Begin(GL.QUADS);

            Max2D.DrawQuad(right0, right1, right3, right2, z);
            Max2D.DrawQuad(left0, left1, left3, left2, z);
            Max2D.DrawQuad(down0, down1, down3, down2, z);
            Max2D.DrawQuad(up0, up1, up3, up2, z);

            GL.End();
            */
        }

        public static void CalculatePoints(LightingBuffer2D buffer) {
            angle0 = angle0.RotToVec(0, 1);
            angle90 = angle90.RotToVec(0 + Mathf.PI / 2);
            angle180 = angle180.RotToVec(0 + Mathf.PI);
            angle270 = angle270.RotToVec(0 - Mathf.PI / 2);

            angle45 = angle45.RotToVec(0 + Mathf.PI / 4);
            angle135 = angle135.RotToVec(0 + Mathf.PI / 2 + Mathf.PI / 4);
            angle225 = angle225.RotToVec(0 + Mathf.PI + Mathf.PI / 4);
            angle315 = angle315.RotToVec(0 - Mathf.PI / 2 + Mathf.PI / 4);
        }

        public static void CalculateOffsets() {
            // Up
            up0.x = angle90.x + angle135.x;
            up0.y = angle90.y + angle135.y;

            up1.x = up0.x + angle45.x;
            up1.y = up0.y + angle45.y;

            up2.x = angle0.x + angle315.x;
            up2.y = angle0.y + angle315.y;

            up3.x = up2.x + angle45.x;
            up3.y = up2.y + angle45.y;

            // Down
            down0.x = angle270.x + angle315.x;
            down0.y = angle270.y + angle315.y;

            down1.x = down0.x + angle225.x;
            down1.y = down0.y + angle225.y;

            down2.x = angle180.x + angle135.x;
            down2.y = angle180.y + angle135.y;

            down3.x = down2.x + angle225.x;
            down3.y = down2.y + angle225.y;
        
            // Left
            left0.x = angle0.x + angle45.x;
            left0.y = angle0.y + angle45.y;

            left1.x = left0.x + angle315.x;
            left1.y = left0.y + angle315.y;

            left2.x = angle270.x + angle225.x;
            left2.y = angle270.y + angle225.y;

            left3.x = left2.x + angle315.x;
            left3.y = left2.y + angle315.y;
                
            // Right
            right0.x = angle180.x + angle225.x;
            right0.y = angle180.y + angle225.y;

            right1.x = right0.x + angle135.x;
            right1.y = right0.y + angle135.y;

            right2.x = angle90.x + angle45.x;
            right2.y = angle90.y + angle45.y;

            right3.x = right2.x + angle135.x;
            right3.y = right2.y + angle135.y;
        }

        static public MeshObject GetMesh() {
            if (meshObject == null) {
                Mesh mesh = new Mesh();

                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();

                int count = 0;

                vertices.Add(right0);
                vertices.Add(right1);
                vertices.Add(right3);
                vertices.Add(right2);

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);

                count += 4;

                vertices.Add(left0);
                vertices.Add(left1);
                vertices.Add(left3);
                vertices.Add(left2);

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);

                count += 4;

                vertices.Add(down0);
                vertices.Add(down1);
                vertices.Add(down3);
                vertices.Add(down2);

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);


                count += 4;

                vertices.Add(up0);
                vertices.Add(up1);
                vertices.Add(up3);
                vertices.Add(up2);

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);

                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();

                meshObject = new MeshObject(mesh);
            }

            return(meshObject);
        }
    }
}
