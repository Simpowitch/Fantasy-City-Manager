using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public class Pass {

        public LightingBuffer2D buffer;
        public LightingSource2D light;
        public LayerSetting layer;
        public int layerID;

        public float lightSizeSquared;
        public float z;

        public List<LightingCollider2D> colliderList;
        public List<LightingCollider2D> layerCollisionList;
        public List<LightingCollider2D> layerMaskList;

        #if UNITY_2017_4_OR_NEWER
            public List<LightingTilemapCollider2D> tilemapList;
        #endif

        public bool drawMask = false;
        public bool drawShadows = false;

        public Material materialWhite;
        public Material materialNormalMap_PixelToLight;
        public Material materialNormalMap_ObjectToLight;

        public Sorting.SortPass sortPass = new Sorting.SortPass();

        public bool Setup(LightingBuffer2D setBuffer, LayerSetting setLayer) {
            // Layer ID
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

            layerCollisionList = LightingCollider2D.GetCollisionList(layerID);
            layerMaskList = LightingCollider2D.GetMaskList(layerID);

            #if UNITY_2017_4_OR_NEWER
                tilemapList = LightingTilemapCollider2D.GetList();
            #endif

            // Draw Mask & Shadows?
            drawMask = (layer.type != LightingLayerType.ShadowOnly);
            drawShadows = (layer.type != LightingLayerType.MaskOnly);

            // Materials
            materialWhite = Lighting2D.materials.GetSpriteMask();
    
            materialNormalMap_PixelToLight = Lighting2D.materials.GetNormalMapSpritePixelToLight();
            materialNormalMap_ObjectToLight = Lighting2D.materials.GetNormalMapSpriteObjectToLight();

            materialNormalMap_PixelToLight.SetFloat("_LightSize", buffer.lightSource.size);
            materialNormalMap_PixelToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
            materialNormalMap_PixelToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

            materialNormalMap_ObjectToLight.SetFloat("_LightSize", buffer.lightSource.size);
            materialNormalMap_ObjectToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
            materialNormalMap_ObjectToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

            sortPass.pass = this;
            
            // Sort
            sortPass.Clear();

            return(true);
        }
    }
}