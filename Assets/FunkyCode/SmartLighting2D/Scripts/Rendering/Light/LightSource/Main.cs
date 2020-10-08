using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public class Main {

         public static void Draw(LightingBuffer2D buffer) {
            Vector2 size = new Vector2(buffer.lightSource.size, buffer.lightSource.size);
            float z = 0;

            Material material = Lighting2D.materials.GetMultiply();

            if (buffer.lightSource != null) {
                UnityEngine.Sprite lightSprite = buffer.lightSource.GetSprite();

                if (lightSprite.texture != null) {
                    material.mainTexture = lightSprite.texture;
                }
            }

            if (buffer.lightSource.applyRotation) {
                Bounds.CalculatePoints(buffer);
                Bounds.CalculateOffsets();

                material.SetPass(0);

                material.color = Color.white;

                Sprite.Draw(Vector2.zero, size, buffer.lightSource.transform.rotation.eulerAngles.z, z, buffer.lightSource.spriteFlipX, buffer.lightSource.spriteFlipY);

                material.color = Color.black;

                Bounds.Draw(buffer, material, z);
                
            } else {
                material.SetPass (0); 

                material.color = Color.white;

                Sprite.Draw(Vector2.zero, size, 0, z, buffer.lightSource.spriteFlipX, buffer.lightSource.spriteFlipY);
            }

            if (buffer.lightSource.angle != 360) {
                Lighting2D.materials.GetAtlasMaterial().SetPass(0);

                GL.Begin(GL.TRIANGLES);

                GL.Color(Color.black);

                Angle.Draw(buffer.lightSource, z);

                GL.End ();
            }
        }
    }
}