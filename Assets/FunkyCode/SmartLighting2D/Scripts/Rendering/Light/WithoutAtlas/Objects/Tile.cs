using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class Tile : Base {
        
       static public void MaskSprite(LightingBuffer2D buffer, LightingTile tile, LayerSetting layerSetting, Material material, Vector2 polyOffset, LightingTilemapCollider2D tilemap, float lightSizeSquared, float z) {
			virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

			if (virtualSpriteRenderer.sprite == null) {
				return;
			}

			material.color = LayerSettingColor.Get(polyOffset, layerSetting, MaskEffect.Lit);

			material.mainTexture = virtualSpriteRenderer.sprite.texture;

			Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, polyOffset, tilemap.transform.lossyScale, 0, z);
			
			material.mainTexture = null;
		}
    }
}