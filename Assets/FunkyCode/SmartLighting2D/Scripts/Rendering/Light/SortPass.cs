using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public class SortedPass {

        public LightingBuffer2D buffer;
        public LightingSource2D light;
        public LightingCollider2D collider;    
        public LayerSetting layer;
        public int layerID;

        public float lightSizeSquared;
        public float z;

        public List<LightingCollider2D> colliderList;

        #if UNITY_2017_4_OR_NEWER
            public List<LightingTilemapCollider2D> tilemapList;
        #endif

        public bool drawMask = false;
        public bool drawShadows = false;

        public Material materialWhite;

        public Material materialNormalMap_PixelToLight;
        public Material materialNormalMap_ObjectToLight;

        public Sorting.SortList sortList = new Sorting.SortList();
        public Sorting.SortObject sortObject;
    
        public PartiallyBatchedCollider batch_collider;              
        public PartiallyBatchedTilemap batch_tilemap;

        #if UNITY_2017_4_OR_NEWER
            public LightingTilemapCollider2D tilemap;
        #endif

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
            z = 0; //  buffer.transform.position.z
        
            colliderList = LightingCollider2D.GetList();

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

            // Sort
            sortList.count = 0;

            return(true);
        }

        public void SortObjects() {
            sortList.Reset();

            for(int id = 0; id < colliderList.Count; id++) {
                // Check If It's In Light Area?
                collider = colliderList[id];

                if ((int)colliderList[id].lightingCollisionLayer != layerID && (int)colliderList[id].lightingMaskLayer != layerID) {
                    continue;
                }

                switch(layer.sorting) {
                    case LightingLayerSorting.ZAxisDown:
                        if (layer.sortingIgnore == LightingLayerSortingIgnore.IgnoreAbove) {
                            if (collider.transform.position.z >= light.transform.position.z) {
                                sortList.Add(collider, -collider.transform.position.z);
                            }
                        } else {
                            sortList.Add(collider, -collider.transform.position.z);
                        }
                        
                        
                    break;

                    case LightingLayerSorting.ZAxisUp:
                        if (layer.sortingIgnore == LightingLayerSortingIgnore.IgnoreAbove) {
                            if (collider.transform.position.z <= light.transform.position.z) {
                                sortList.Add(collider, collider.transform.position.z);
                            }
                        } else {
                            sortList.Add(collider, collider.transform.position.z);
                        }

                    break;

                    case LightingLayerSorting.YAxisDown:
                        sortList.Add(collider, -collider.transform.position.y);
                    break;

                    case LightingLayerSorting.YAxisUp:
                        sortList.Add(collider, collider.transform.position.y);
                    break;

                    case LightingLayerSorting.DistanceToLight:
                        sortList.Add(collider, -Vector2.Distance(collider.transform.position, light.transform.position));
                    break;
                }
            }

            #if UNITY_2017_4_OR_NEWER

                for(int id = 0; id < tilemapList.Count; id++) {
                    AddTilemap(tilemapList[id]);
                }

            #endif

            sortList.Sort();
        }

        #if UNITY_2017_4_OR_NEWER

            public void AddTilemap(LightingTilemapCollider2D id) {
                if (id.isometric == null) {
                    return;
                }
                
                TilemapProperties properties = id.rectangle.Properties;
                Vector2 offset = -buffer.lightSource.transform.position;

                int tilemapSize = Rectangle.Light.GetSize(id, buffer);
                Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);
        
                for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                    for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                        if (x < 0 || y < 0) {
                            continue;
                        }

                        if (x >= properties.area.size.x || y >= properties.area.size.y) {
                            continue;
                        }

                        LightingTile tile = id.rectangle.map.map[x, y];
                        if (tile == null) {
                            continue;
                        }

                        if (tile.GetOriginalSprite() == null) {
                            return;
                        }

                        Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

                        tilePosition += offset;

                        if (tile.InRange(tilePosition, 2 + light.size / 2)) {
                            continue;
                        }
                        
                        switch(layer.sorting) {
                            case LightingLayerSorting.ZAxisDown:
                                sortList.Add(collider, -id.transform.position.z);
                            break;

                            case LightingLayerSorting.ZAxisUp:
                                sortList.Add(collider, id.transform.position.z);
                            break;
                            
                            case LightingLayerSorting.YAxisDown:
                                sortList.Add(id, tile, -tilePosition.y, tilePosition);
                            break;

                            case LightingLayerSorting.YAxisUp:
                                sortList.Add(id, tile, tilePosition.y, tilePosition);
                            break;

                            case LightingLayerSorting.DistanceToLight:
                                sortList.Add(id, tile,  -Vector2.Distance(tilePosition - offset, light.transform.position), tilePosition);
                            break;
                        }
                    }	
                }
            }
        
        #endif
    }
}