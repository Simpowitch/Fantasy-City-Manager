using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public class Angle {

        public static void Draw(LightingSource2D lightSource, float z) {
            float size = lightSource.Buffer.lightSource.size;

            float squaredSize = Mathf.Sqrt((float)((size * size) + (size * size)));
            float shadowAngle = 360 - lightSource.angle;

            int step = 5;

            for(int i = 0; i < shadowAngle; i += step) {
                GL.Color(Color.black);
                float rotation = i - shadowAngle / 2 - 90 + lightSource.transform.eulerAngles.z;
                float angle1 = Mathf.Deg2Rad * (rotation);
                float angle2 = Mathf.Deg2Rad * (rotation + step);

                Vector2 pos0 = Vector2.zero;
                Vector2 pos1 = new Vector2(Mathf.Cos(angle1) * squaredSize, Mathf.Sin(angle1) * squaredSize);
                Vector2 pos2  = new Vector2(Mathf.Cos(angle2) * squaredSize, Mathf.Sin(angle2) * squaredSize);

                GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                GL.Vertex3(pos0.x, pos0.y, z);

                GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                GL.Vertex3(pos1.x, pos1.y, z);

                GL.TexCoord3(Max2D.texCoord.x, Max2D.texCoord.y, 0);
                GL.Vertex3(pos2.x, pos2.y, z);

                if (i == 0) {
                    GL.Color(Color.white);
                    
                    float penumbra = -lightSource.outerAngle;
                    angle1 = Mathf.Deg2Rad * (rotation);
                    angle2 = Mathf.Deg2Rad * (rotation + penumbra);

                    pos0 = Vector2.zero;
                    pos1 = new Vector2(Mathf.Cos(angle1) * squaredSize, Mathf.Sin(angle1) * squaredSize);
                    pos2 = new Vector2(Mathf.Cos(angle2) * squaredSize, Mathf.Sin(angle2) * squaredSize);

                    GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
                    GL.Vertex3(pos0.x, pos0.y, z);

                    GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y, 0);
                    GL.Vertex3(pos2.x, pos2.y, z);
                    
                    GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
                    GL.Vertex3(pos1.x, pos1.y, z);

                } else if (i + step >= shadowAngle) {
                    GL.Color(Color.white);
                    
                    float penumbra = lightSource.outerAngle;
                    angle1 = Mathf.Deg2Rad * (rotation + 5);
                    angle2 = Mathf.Deg2Rad * (rotation + penumbra + 5);

                    pos0 = Vector2.zero;
                    pos1 = new Vector2(Mathf.Cos(angle1) * squaredSize, Mathf.Sin(angle1) * squaredSize);
                    pos2  = new Vector2(Mathf.Cos(angle2) * squaredSize, Mathf.Sin(angle2) * squaredSize);

                    GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.y, 0);
                    GL.Vertex3(pos0.x, pos0.y, z);

                    GL.TexCoord3(Penumbra.uvRect.width, Penumbra.uvRect.y, 0);
                    GL.Vertex3(pos2.x, pos2.y, z);
                    
                    GL.TexCoord3(Penumbra.uvRect.x, Penumbra.uvRect.height, 0);
                    GL.Vertex3(pos1.x, pos1.y, z);
                }
            }
        }
    }
}