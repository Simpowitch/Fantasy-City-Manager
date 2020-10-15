using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Day {

    public class NoSort {

        static public void Draw(Camera camera, Vector2 offset, float z, LightingLayerSetting nightLayer) {
            int layer = (int)nightLayer.layer;

            if (Lighting2D.atlasSettings.lightingSpriteAtlas) {

            } else {
               Rendering.Day.WithoutAtlas.NoSort.Draw(camera, offset, z, nightLayer);
            }
        }
    }
}
