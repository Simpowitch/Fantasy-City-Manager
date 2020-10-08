using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day.WithoutAtlas {

    public class SpriteRendererShadow {

        static public void Draw(Camera camera, Vector2 offset, float z, int layer) {
            Material material = Lighting2D.materials.GetWhiteSprite();

            material.color = Color.black;

            List<DayLightingCollider2D> colliderList = DayLightingCollider2D.GetList();
        
            for(int idd = 0; idd < colliderList.Count; idd++) {
                DayLightingCollider2D id = colliderList[idd];

                if ((int)id.collisionDayLayer != layer) {
                    continue;
                }
            
                if (id.shape.colliderType != DayLightingCollider2D.ColliderType.Sprite) {
                    continue;
                }

                if (id.InAnyCamera() == false) {
					continue;
				}
        
                VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
                virtualSpriteRenderer.sprite = id.shape.GetSprite();

                if (virtualSpriteRenderer.sprite == null) {
                    continue;
                }
                                    
                float x = id.transform.position.x + (float)offset.x;
                float y = id.transform.position.y + (float)offset.y;

                float rot = Lighting2D.dayLightingSettings.direction * Mathf.Deg2Rad;

                float sunHeight = Lighting2D.dayLightingSettings.height;

                x += Mathf.Cos(rot) * id.shape.height * sunHeight;
                y += Mathf.Sin(rot) * id.shape.height * sunHeight;

                material.mainTexture = virtualSpriteRenderer.sprite.texture;

                Vector2 scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

                Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(id.spriteMeshObject, material, virtualSpriteRenderer, new Vector2(x, y), scale, id.transform.rotation.eulerAngles.z, z);
            }

            material.color = Color.white;
        }
    }
}