using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Day.WithoutAtlas {

    public class NoSort {

        static public void Draw(Camera camera, Vector2 offset, float z, LightingLayerSetting nightLayer) {
            int layer = (int)nightLayer.layer;

            
            
            List<DayLightingCollider2D> colliderList = DayLightingCollider2D.GetList();
            int colliderCount = colliderList.Count;

            bool drawShadows = nightLayer.type != LightingLayerSettingType.MaskOnly;
            bool drawMask = nightLayer.type != LightingLayerSettingType.ShadowsOnly;

            if (drawShadows) {
                Lighting2D.materials.GetShadowBlur().SetPass (0);
                
                GL.Begin(GL.TRIANGLES);


                for(int i = 0; i < colliderCount; i++) {
                    DayLightingCollider2D id = colliderList[i];
                    
                    if ((int)id.collisionDayLayer != layer) {
                        continue;
                    }

                    WithoutAtlas.Shadow.Draw(id, camera, offset, z);                
                }

                GL.End();

                    
                for(int idd = 0; idd < colliderList.Count; idd++) {
                    DayLightingCollider2D id = colliderList[idd];

                    if ((int)id.collisionDayLayer != layer) {
                        continue;
                    }
                
                    WithoutAtlas.SpriteRendererShadow.Draw(id, camera, offset, z);
                }
            }

            
            if (drawMask) {
                for(int i = 0; i < colliderCount; i++) {
                    DayLightingCollider2D id = colliderList[i];

                    if ((int)id.maskDayLayer != layer) {
                        continue;
                    }

                    WithoutAtlas.SpriteRenderer2D.Draw(id, camera, offset, z);
                }
            }
        }
    }
}
