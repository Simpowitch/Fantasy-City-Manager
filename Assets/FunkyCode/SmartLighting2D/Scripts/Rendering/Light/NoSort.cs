using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public static class NoSort {
		private static Pass pass = new Pass();

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

        public class Pass {
            public LightingBuffer2D buffer;
			public LightingSource2D light;
            public LayerSetting layer;
			public int layerID;

            public float lightSizeSquared;
            public float z;

            public List<LightingCollider2D> colliderList;

            #if UNITY_2017_4_OR_NEWER
                public List<LightingTilemapCollider2D> tilemapList;
            #endif

            public bool drawMask;
            public bool drawShadows;

            public Material materialWhite;

			public Material materialNormalMap_PixelToLight;
			public Material materialNormalMap_ObjectToLight;

			public bool Setup(LightingBuffer2D setBuffer, LayerSetting setLayer) {
				layerID = setLayer.GetLayerID();
				if (layerID < 0) {
					return(false);
				}

				buffer = setBuffer;
				layer = setLayer;

				// Calculation Setup
				light = buffer.lightSource;
				lightSizeSquared = Mathf.Sqrt(light.size * light.size + light.size * light.size);
				z = 0;

				colliderList = LightingCollider2D.GetList();

				#if UNITY_2017_4_OR_NEWER
					tilemapList = LightingTilemapCollider2D.GetList();
				#endif

				// Draw Mask & Shadows
				drawMask = (layer.type != LightingLayerType.ShadowOnly);
				drawShadows = (layer.type != LightingLayerType.MaskOnly);

				// Materials
				materialWhite = Lighting2D.materials.GetWhiteSprite();

				materialNormalMap_PixelToLight = Lighting2D.materials.GetNormalMapSpritePixelToLight();
				materialNormalMap_ObjectToLight = Lighting2D.materials.GetNormalMapSpriteObjectToLight();

				materialNormalMap_PixelToLight.SetFloat("_LightSize", buffer.lightSource.size);
				materialNormalMap_PixelToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
				materialNormalMap_PixelToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

				materialNormalMap_ObjectToLight.SetFloat("_LightSize", buffer.lightSource.size);
				materialNormalMap_ObjectToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
				materialNormalMap_ObjectToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

				return(true);
			}
        }
    }
}