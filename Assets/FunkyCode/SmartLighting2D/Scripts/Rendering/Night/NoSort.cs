using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {

    public class NoSort {

        static public void Draw(Camera camera, Vector2 offset, float z, LightingLayerSetting nightLayer) {
            int layer = (int)nightLayer.layer;

            if (Lighting2D.atlasSettings.lightingSpriteAtlas) {
                Rendering.Night.WithAtlas.NoSort.Draw(camera, offset, z, layer);
            } else {
                Rendering.Night.WithoutAtlas.NoSort.Draw(camera, offset, z, layer);
            }
        }
    }
}
