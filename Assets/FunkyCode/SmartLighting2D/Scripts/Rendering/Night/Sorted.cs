using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
        
    public class Sorted {

        static SortedPass pass = new SortedPass();

        static public void Draw(Camera camera, Vector2 offset, float z, LightingLayerSetting nightLayer) {
            if (pass.Setup(nightLayer) == false) {
                return;
            }

            pass.SortObjects();

            if (Lighting2D.atlasSettings.lightingSpriteAtlas) {

            } else {
                Rendering.Night.WithoutAtlas.Sorted.Draw(camera, offset, z, pass);
            }
        }
    }
}