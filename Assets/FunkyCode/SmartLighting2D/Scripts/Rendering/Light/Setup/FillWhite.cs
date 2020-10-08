using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
    public class FillWhite {
        static public Rect uvRect = new Rect();
        static public bool highQuality = true;

        static public void Calculate() {
            LightingManager2D manager = LightingManager2D.Get();
            
            Sprite fillSprite = Lighting2D.materials.GetAtlasWhiteMaskSprite();
        
            highQuality = Lighting2D.commonSettings.highQualityShadows == LightingSettings.QualitySettings.ShadowQuality.Detailed;

            if (fillSprite != null) {
                Rect spriteRect = fillSprite.textureRect;

                uvRect.x = spriteRect.x / fillSprite.texture.width;
                uvRect.y = spriteRect.y / fillSprite.texture.height;
                uvRect.width = spriteRect.width / fillSprite.texture.width;
                uvRect.height = spriteRect.height / fillSprite.texture.height;

                uvRect.x += uvRect.width / 2;
                uvRect.y += uvRect.height / 2;
                
                Max2D.texCoord = new Vector2(uvRect.x, uvRect.y);
            }
        }
    }
}