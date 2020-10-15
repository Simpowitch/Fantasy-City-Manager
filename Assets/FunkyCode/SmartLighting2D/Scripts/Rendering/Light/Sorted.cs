using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public class Sorted {
        static SortedPass pass = new SortedPass();

        static public void Draw(LightingBuffer2D buffer, LayerSetting layer) {
            if (pass.Setup(buffer, layer) == false) {
                return;
            }
       
            pass.SortObjects();

            if (Lighting2D.atlasSettings.lightingSpriteAtlas && AtlasSystem.Manager.GetAtlasPage() != null) {
                WithAtlas.Sorted.Draw(pass);
            } else {
                WithoutAtlas.Sorted.Draw(pass);
            }
        }
    }
}