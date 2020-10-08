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
       
            pass.SortObjects();

            if (Lighting2D.atlasSettings.lightingSpriteAtlas && AtlasSystem.Manager.GetAtlasPage() != null) {
                WithAtlas.Sorted.Draw(pass);
            } else {
                WithoutAtlas.Sorted.Draw(pass);
            }
        }

        public class Pass {
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

            public ColliderDepthList list = new ColliderDepthList();
            public ColliderDepth depth;
        
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
                materialWhite = Lighting2D.materials.GetWhiteSprite();
      
                materialNormalMap_PixelToLight = Lighting2D.materials.GetNormalMapSpritePixelToLight();
                materialNormalMap_ObjectToLight = Lighting2D.materials.GetNormalMapSpriteObjectToLight();

                materialNormalMap_PixelToLight.SetFloat("_LightSize", buffer.lightSource.size);
				materialNormalMap_PixelToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
				materialNormalMap_PixelToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

				materialNormalMap_ObjectToLight.SetFloat("_LightSize", buffer.lightSource.size);
				materialNormalMap_ObjectToLight.SetFloat("_LightIntensity", buffer.lightSource.bumpMap.intensity);
				materialNormalMap_ObjectToLight.SetFloat("_LightZ", buffer.lightSource.bumpMap.depth);

                // Sort
                list.count = 0;

                return(true);
            }

            public void SortObjects() {
                list.Reset();

                for(int id = 0; id < colliderList.Count; id++) {
                    // Check If It's In Light Area?
                    collider = colliderList[id];

                    if ((int)colliderList[id].lightingCollisionLayer != layerID && (int)colliderList[id].lightingMaskLayer != layerID) {
                        continue;
                    }

                    switch(layer.sorting) {
                        case LightingLayerSorting.YAxisDown:
                            list.Add(collider, -collider.transform.position.y);
                        break;

                        case LightingLayerSorting.YAxisUp:
                            list.Add(collider, collider.transform.position.y);
                        break;

                        case LightingLayerSorting.DistanceToLight:
                            list.Add(collider, -Vector2.Distance(collider.transform.position, light.transform.position));
                        break;
                    }
                }

                #if UNITY_2017_4_OR_NEWER

                    for(int id = 0; id < tilemapList.Count; id++) {
                        AddTilemap(tilemapList[id]);
                    }

                #endif

                list.Sort();
            }

            #if UNITY_2017_4_OR_NEWER

                public void AddTilemap(LightingTilemapCollider2D id) {
                    if (id.rectangleMap == null) {
                        return;
                    }
                    
                    if (id.rectangleMap.map == null) {
                        return;
                    }
                
                    Vector2 polyOffset;
                    Vector2 tilemapOffset = Base.GetTilemapOffset(id);
                    Vector2 posScale = Base.GetPositionScale(id);
                    Vector2Int tilemapLightPosition = Base.GetTilemapLightPosition(id, buffer);
                    int tilemapSize = Base.GetTilemapSize(id, buffer);

                    Vector2 offset = -buffer.lightSource.transform.position;
           
                    for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                        for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                            if (x < 0 || y < 0) {
                                continue;
                            }

                            if (x >= id.properties.area.size.x || y >= id.properties.area.size.y) {
                                continue;
                            }

                            LightingTile tile = id.rectangleMap.map[x, y];
                            if (tile == null) {
                                continue;
                            }

                            if (tile.GetOriginalSprite() == null) {
                                return;
                            }

                            polyOffset.x = x + tilemapOffset.x;
                            polyOffset.y = y + tilemapOffset.y;

                            polyOffset.x *= posScale.x;
                            polyOffset.y *= posScale.y;

                            if (tile.InRange(polyOffset, light.transform.position, 2 + light.size / 2)) {
                                continue;
                            }
                            
                            polyOffset += offset;

                            switch(layer.sorting) {
                                case LightingLayerSorting.YAxisDown:
                                    list.Add(id, tile, -(float)polyOffset.y, polyOffset);
                                break;

                                case LightingLayerSorting.YAxisUp:
                                    list.Add(id, tile, (float)polyOffset.y, polyOffset);
                                break;

                                case LightingLayerSorting.DistanceToLight:
                                    list.Add(id, tile,  -Vector2.Distance(polyOffset - offset, light.transform.position), polyOffset);
                                break;
                            }
                        }	
                    }
                }
            
            #endif
        }
    }
}