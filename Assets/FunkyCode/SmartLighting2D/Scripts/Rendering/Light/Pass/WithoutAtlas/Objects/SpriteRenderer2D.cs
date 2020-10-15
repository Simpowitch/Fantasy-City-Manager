using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class SpriteRenderer2D : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

            foreach(LightingColliderShape shape in id.shapes) {
                UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

                Sprite sprite = shape.spriteShape.GetOriginalSprite();
                if (sprite == null || spriteRenderer == null) {
                    continue;
                }

                Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;

                material.color = LayerSettingColor.Get(position, layerSetting, id.maskEffect);

                material.mainTexture = sprite.texture;

                Rendering.Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, shape.transform2D.scale, shape.transform2D.rotation, z);	
            }
		}

        public static void MaskNormalMap(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

            Texture normalTexture = id.normalMapMode.GetBumpTexture();

            if (normalTexture == null) {
                return;
            }

            float rotation;

            material.SetTexture("_Bump", normalTexture);

            foreach(LightingColliderShape shape in id.shapes) {
                UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

                if (spriteRenderer == null) {
                    continue;
                }

                Sprite sprite = shape.spriteShape.GetOriginalSprite();
                if (sprite == null) {
                    continue;
                }
               
                Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;

                material.mainTexture = sprite.texture;
                material.color = LayerSettingColor.Get(position, layerSetting, id.maskEffect);
                
                float color = material.color.r;
                
                switch(id.normalMapMode.type) {
                    case NormalMapType.ObjectToLight:
                        rotation = Mathf.Atan2(buffer.lightSource.transform2D.position.y - shape.transform2D.position.y, buffer.lightSource.transform2D.position.x - shape.transform2D.position.x);
                        rotation -= Mathf.Deg2Rad * (shape.transform2D.rotation);
                        
                        material.SetFloat("_LightRX", Mathf.Cos(rotation) * 2);
                        material.SetFloat("_LightRY", Mathf.Sin(rotation) * 2);
                        material.SetFloat("_LightColor",  color);

                    break;

                    case NormalMapType.PixelToLight:
                        material.SetFloat("_LightColor",  color);
                    
                        rotation = shape.transform2D.rotation * Mathf.Deg2Rad;

                        Vector2 sc = shape.transform2D.scale;
                        sc = sc.normalized;

                        material.SetFloat("_LightX", Mathf.Cos(rotation) * sc.x );
                        material.SetFloat("_LightY", Mathf.Cos(rotation) * sc.y );

                    break;
                }

                Rendering.Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, shape.transform2D.scale, shape.transform2D.rotation, z);
            }
        }
    }
}