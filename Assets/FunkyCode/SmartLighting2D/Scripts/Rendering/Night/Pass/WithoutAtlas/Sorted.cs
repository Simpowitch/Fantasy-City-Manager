using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {

    public class Sorted {
        
         static public void Draw(Camera camera, Vector2 offset, float z, SortedPass pass) {
             for(int id = 0; id < pass.sortList.count; id++) {
                Sorting.SortObject sortObject = pass.sortList.list[id];

                switch(sortObject.type) {
                    case Sorting.SortObject.Type.Room:
                        LightingRoom2D room = (LightingRoom2D)sortObject.lightObject;

                        if (room != null) {
                            WithoutAtlas.Room.Draw(room, camera, offset, z);
                        }

                    break;

                    case Sorting.SortObject.Type.LightSprite:
                        LightingSpriteRenderer2D lightSprite = (LightingSpriteRenderer2D)sortObject.lightObject;

                        if (lightSprite != null) {
                            WithoutAtlas.SpriteRenderer2D.Draw(lightSprite, camera, offset, z);
                        }

                    break;

                    case Sorting.SortObject.Type.Light:
                        LightingSource2D light = (LightingSource2D)sortObject.lightObject;

                        if (light != null) {
                            Rendering.Night.LightSource.Draw(light, camera, offset, z);
                        }

                    break;
                }
            }
         }
    }
}