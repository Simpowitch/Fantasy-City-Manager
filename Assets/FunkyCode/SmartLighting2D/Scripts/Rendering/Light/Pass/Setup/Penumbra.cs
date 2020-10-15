using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
    static public class Penumbra {
        static public Rect uvRect = new Rect();
        static public Vector2 size;
 
        static Sprite sprite = null;

        public static void Calculate() {
            LightingManager2D manager = LightingManager2D.Get();
            
            sprite = Lighting2D.materials.GetAtlasPenumbraSprite();

            if (sprite == null || sprite.texture == null) {
                return;
            }

            Rect spriteRect = sprite.textureRect;
            int atlasSize = AtlasSystem.Manager.GetAtlasPage().atlasSize / 2;


            uvRect.x = spriteRect.x / sprite.texture.width;
            uvRect.y = spriteRect.y / sprite.texture.height;

            size.x = ((float)spriteRect.width) / sprite.texture.width;
            size.y = ((float)spriteRect.height) / sprite.texture.height;

            uvRect.width = spriteRect.width / 2 / sprite.texture.width;
            uvRect.height = spriteRect.height / 2 / sprite.texture.height;

            uvRect.width += uvRect.x;
            uvRect.height += uvRect.y;

            uvRect.x += 1f / atlasSize;
            uvRect.y += 1f / atlasSize;
            uvRect.width -= 1f / atlasSize;
            uvRect.height -= 1f / atlasSize;


        }
    }
}