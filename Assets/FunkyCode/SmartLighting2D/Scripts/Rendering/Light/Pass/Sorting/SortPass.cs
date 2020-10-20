using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Sorting {

    public class SortPass {

        public Sorting.SortList sortList = new Sorting.SortList();
        public Sorting.SortObject sortObject;
        public Rendering.Light.Pass pass;

        public void Clear() {
            sortList.count = 0;
        }
        
        public void SortObjects() {
            if (pass == null) {
                return;
            }
            
            sortList.Reset();

            for(int id = 0; id < pass.colliderList.Count; id++) {
                // Check If It's In Light Area?
                LightingCollider2D collider = pass.colliderList[id];

                if ((int)pass.colliderList[id].lightingCollisionLayer != pass.layerID && (int)pass.colliderList[id].lightingMaskLayer != pass.layerID) {
                    continue;
                }

                switch(pass.layer.sorting) {
                    case LightingLayerSorting.ZAxisDown:
                        if (pass.layer.sortingIgnore == LightingLayerSortingIgnore.IgnoreAbove) {
                            if (collider.transform.position.z >= pass.light.transform.position.z) {
                                sortList.Add(collider, -collider.transform.position.z);
                            }
                        } else {
                            sortList.Add(collider, -collider.transform.position.z);
                        }
                        
                        
                    break;

                    case LightingLayerSorting.ZAxisUp:
                        if (pass.layer.sortingIgnore == LightingLayerSortingIgnore.IgnoreAbove) {
                            if (collider.transform.position.z <= pass.light.transform.position.z) {
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
                        sortList.Add(collider, -Vector2.Distance(collider.transform.position, pass.light.transform.position));
                    break;
                }
            }

            #if UNITY_2017_4_OR_NEWER

                for(int id = 0; id < pass.tilemapList.Count; id++) {
                    AddTilemap(pass.tilemapList[id]);
                }

            #endif

            sortList.Sort();
        }
     
        #if UNITY_2017_4_OR_NEWER

            public void AddTilemap(LightingTilemapCollider2D id) {

                foreach(LightingTile tile in id.rectangle.mapTiles) {
                    if (tile.GetOriginalSprite() == null) {
                        return;
                    }

                    Vector2 tilePosition = Rectangle.GetTilePosition(tile.position.x, tile.position.y, id);

                    // Optimization broke Shadoe Engine light offset
                    //if (tile.InRange(tilePosition, 2 + light.size / 2)) {
                    //    continue;
                    //}

                    switch(pass.layer.sorting) {
                        case LightingLayerSorting.ZAxisDown:
                            sortList.Add(id, tile, -id.transform.position.z, tilePosition);
                        break;

                        case LightingLayerSorting.ZAxisUp:
                            sortList.Add(id, tile, id.transform.position.z, tilePosition);
                        break;
                        
                        case LightingLayerSorting.YAxisDown:
                            sortList.Add(id, tile, -tilePosition.y, tilePosition);
                        break;

                        case LightingLayerSorting.YAxisUp:
                            sortList.Add(id, tile, tilePosition.y, tilePosition);
                        break;

                        case LightingLayerSorting.DistanceToLight:
                            sortList.Add(id, tile,  -Vector2.Distance(tilePosition, pass.light.transform.position), tilePosition);
                        break;
                    
                    }	
                }
            }
        
        #endif
    }

}