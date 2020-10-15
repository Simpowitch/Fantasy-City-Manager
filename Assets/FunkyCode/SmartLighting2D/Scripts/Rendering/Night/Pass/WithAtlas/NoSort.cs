using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithAtlas {

    public class NoSort {
        
        public static void Draw(Camera camera, Vector2 offset, float z, int layer) {
            Rendering.Night.WithAtlas.SpriteRenderer.Draw(camera, offset, z, layer);

            foreach (LightingSource2D id in LightingSource2D.GetList()) {
                if ((int)id.nightLayer != layer) {
                    continue;
                }

               Rendering.Night.LightSource.Draw(id, camera, offset, z);
            }
        }
    }
}