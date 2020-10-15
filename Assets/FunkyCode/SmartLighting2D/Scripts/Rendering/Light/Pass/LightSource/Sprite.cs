using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public class Sprite {

        static public void Draw(Vector2 pos, Vector2 size, float rot, float z, bool flipX, bool flipY) {
            Vector2 scale = new Vector2(size.x, size.y);

            if (flipY) {
                scale.y = -scale.y;
            }

            if (flipX) {
                scale.x = -scale.x;
            }

            // Vector3 matrixPosition = new Vector3(0, 0, z);
            // Quaternion matrixRotation = Quaternion.Euler(0, 0, rot);
            // Vector3 matrixScale = new Vector3(scale.x, scale.y, 1);
            //Graphics.DrawMeshNow(GetMesh(), Matrix4x4.TRS(matrixPosition, matrixRotation, matrixScale));

            Rendering.Universal.WithoutAtlas.Texture.Draw( new Vector2(0, 0), scale, rot, z);
        }
    }
}