using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {

    public class SortedPass {
        
        public Sorting.SortList sortList = new Sorting.SortList();
        public Sorting.SortObject sortObject;
        public int layerId;
        public LightingLayerSetting layer;

        public void SortObjects() {
            sortList.Reset();

            List<LightingSource2D> lightList = LightingSource2D.GetList();
            for(int id = 0; id < lightList.Count; id++) {
                LightingSource2D light = lightList[id];

                if ((int)light.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {
                    case LightingLayerSettingSorting.ZAxisDown:
                        sortList.Add((object)light, Sorting.SortObject.Type.Light, - light.transform.position.z);
                    break;

                    case LightingLayerSettingSorting.ZAxisUp:
                        sortList.Add((object)light, Sorting.SortObject.Type.Light, light.transform.position.z);
                    break;
                }
            }

            List<LightingRoom2D> roomList = LightingRoom2D.GetList();
            for(int id = 0; id < roomList.Count; id++) {
                LightingRoom2D room = roomList[id];

                if ((int)room.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {

                    case LightingLayerSettingSorting.ZAxisDown:
                        sortList.Add((object)room, Sorting.SortObject.Type.Room, - room.transform.position.z);
                    break;

                    case LightingLayerSettingSorting.ZAxisUp:
                        sortList.Add((object)room, Sorting.SortObject.Type.Room, room.transform.position.z);
                    break;
                }
            }

            List<LightingSpriteRenderer2D> spriteList = LightingSpriteRenderer2D.GetList();
            for(int id = 0; id < spriteList.Count; id++) {
                LightingSpriteRenderer2D lightSprite = spriteList[id];

                if ((int)lightSprite.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {

                    case LightingLayerSettingSorting.ZAxisDown:
                        sortList.Add((object)lightSprite, Sorting.SortObject.Type.LightSprite, - lightSprite.transform.position.z);
                    break;

                    case LightingLayerSettingSorting.ZAxisUp:
                        sortList.Add((object)lightSprite, Sorting.SortObject.Type.LightSprite, lightSprite.transform.position.z);
                    break;
                }
            }

            sortList.Sort();
        }


        public bool Setup(LightingLayerSetting slayer) {
            if (slayer.layer < 0) {
                return(false);
            }
            layerId = (int)slayer.layer;
            layer = slayer;

            return(true);
        }
    }
}