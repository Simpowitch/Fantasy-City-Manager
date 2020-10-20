using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Universal.WithAtlas {
	
	public class Sprite : Base {
		
	    // Rendering.Light (because of Color its not universal)
        static public void Draw(VirtualSpriteRenderer spriteRenderer, LayerSetting layerSetting, MaskEffect maskEffect, Vector2 position, Vector2 size, float rotation, float z = 0f) {
			UnityEngine.Sprite sprite = spriteRenderer.sprite;
			if (spriteRenderer == null || sprite == null || sprite.texture == null) {
				return;
			}

			SpriteTransform spriteTransform = new SpriteTransform(spriteRenderer, position, size, rotation);

			GL.Color(LayerSettingColor.Get(position, layerSetting, maskEffect));

			Texture.Draw(spriteTransform.position, spriteTransform.scale, spriteTransform.uv, spriteTransform.rotation, z);
		}

        static public void Draw(VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 size, float rotation, float z) {
			UnityEngine.Sprite sprite = spriteRenderer.sprite;
			if (sprite == null || sprite.texture == null) {
				return;
			}

			SpriteTransform spriteTransform = new SpriteTransform(spriteRenderer, position, size, rotation);

			Texture.Draw(spriteTransform.position, spriteTransform.scale, spriteTransform.uv, spriteTransform.rotation, z);
		}
    }
}