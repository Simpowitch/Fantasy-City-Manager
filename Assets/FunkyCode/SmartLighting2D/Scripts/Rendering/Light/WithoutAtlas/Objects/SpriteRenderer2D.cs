using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class SpriteRenderer2D : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

			UnityEngine.SpriteRenderer spriteRenderer = id.shape.spriteShape.GetSpriteRenderer();

            Sprite sprite = id.shape.spriteShape.GetOriginalSprite();
            if (sprite == null || spriteRenderer == null) {
                return;
            }

            Vector2 position = id.transform2D.position - buffer.lightSource.transform2D.position;

            material.color = LayerSettingColor.Get(position, layerSetting, id.maskEffect);

            material.mainTexture = sprite.texture;

            Rendering.Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, id.transform2D.scale, id.transform2D.rotation, z);	
        }

        public static void MaskNormalMap(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

			UnityEngine.SpriteRenderer spriteRenderer = id.shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null) {
                return;
            }

            Sprite sprite = id.shape.spriteShape.GetOriginalSprite();
            if (sprite == null) {
                return;
            }

            Texture normalTexture = id.normalMapMode.GetBumpTexture();

            if (normalTexture == null) {
                return;
            }

            Vector2 position = id.transform2D.position - buffer.lightSource.transform2D.position;

            material.color = LayerSettingColor.Get(position, layerSetting, id.maskEffect);

            material.mainTexture = sprite.texture;
            material.SetTexture("_Bump", normalTexture);

            float color = 1;

            if (layerSetting.effect == LightingLayerEffect.AboveLit) {
                color = (id.transform.position.y - buffer.lightSource.transform.position.y) * 2 + 0.75f;
            }
            
            if (color < 0) {
               color = 0;
            }

            if (color > 1) {
                color = 1;
            }

            float rotation;
 
            switch(id.normalMapMode.type) {
                case NormalMapType.ObjectToLight:
                    rotation = Mathf.Atan2(buffer.lightSource.transform.position.y - id.transform.transform.position.y, buffer.lightSource.transform.position.x - id.transform.position.x);
                    rotation -= Mathf.Deg2Rad * (id.transform.eulerAngles.z);
                    
                    material.SetFloat("_LightRX", Mathf.Cos(rotation) * 2);
                    material.SetFloat("_LightRY", Mathf.Sin(rotation) * 2);
                    material.SetFloat("_LightColor",  color);

                break;

                case NormalMapType.PixelToLight:
                    material.SetFloat("_LightColor",  color);
                
                    rotation = id.transform.eulerAngles.z * Mathf.Deg2Rad;

                    Vector2 sc = id.transform.lossyScale;
                    sc = sc.normalized;

                    material.SetFloat("_LightX", Mathf.Cos(rotation) * sc.x );
                    material.SetFloat("_LightY", Mathf.Cos(rotation) * sc.y );

                break;
            }

            Rendering.Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, id.transform2D.scale, id.transform2D.rotation, z);
        }
    }
}