using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public static class NoSort {
		
		private static NoSortPass pass = new NoSortPass();

        public static void Draw(LightingBuffer2D buffer, LayerSetting layer) {
			if (pass.Setup(buffer, layer) == false) {
                return;
            }

			if (Lighting2D.atlasSettings.lightingSpriteAtlas && AtlasSystem.Manager.GetAtlasPage() != null) {
				WithAtlas.NoSort.Draw(pass);
			} else {
				WithoutAtlas.NoSort.Draw(pass);
			}
		}

    }
}