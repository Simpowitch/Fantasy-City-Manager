using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day.WithoutAtlas {

    public class SpriteRendererShadow {
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

        static public void Draw(DayLightingCollider2D id, Camera camera, Vector2 offset, float z) {
            if (id.mainShape.colliderType != DayLightingCollider2D.ColliderType.Sprite) {
                return;
            }

            if (id.InAnyCamera() == false) {
                return;
            }

            Material material = Lighting2D.materials.GetSpriteMask();
            material.color = Color.black;

            foreach(DayLightingColliderShape shape in id.shapes) {
                
                SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();
                if (spriteRenderer == null) {
                    continue;
                }
                
                virtualSpriteRenderer.sprite = spriteRenderer.sprite;

                if (virtualSpriteRenderer.sprite == null) {
                    continue;
                }
                                    
                float x = id.transform.position.x + (float)offset.x;
                float y = id.transform.position.y + (float)offset.y;

                float rot = -Lighting2D.dayLightingSettings.direction * Mathf.Deg2Rad;

                float sunHeight = Lighting2D.dayLightingSettings.height;

                x += Mathf.Cos(rot) * id.mainShape.height * sunHeight;
                y += Mathf.Sin(rot) * id.mainShape.height * sunHeight;

                material.mainTexture = virtualSpriteRenderer.sprite.texture;

                Vector2 scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

                Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(id.spriteMeshObject, material, virtualSpriteRenderer, new Vector2(x, y), scale, id.transform.rotation.eulerAngles.z, z);
            }

            material.color = Color.white;
        }
    }
}