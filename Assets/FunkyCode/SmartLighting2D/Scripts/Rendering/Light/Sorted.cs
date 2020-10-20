using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public class Sorted {
        static Pass pass = new Pass();

        static public void Draw(LightingBuffer2D buffer, LayerSetting layer) {
            if (pass.Setup(buffer, layer) == false) {
                return;
            }
       
            pass.sortPass.SortObjects();

            ShadowEngine.SetPass(buffer.lightSource, layer);

            if (Lighting2D.atlasSettings.lightingSpriteAtlas && AtlasSystem.Manager.GetAtlasPage() != null) {
                WithAtlas.Sorted.Draw(pass);
            } else {
                WithoutAtlas.Sorted.Draw(pass);
            }
        }
    }
}