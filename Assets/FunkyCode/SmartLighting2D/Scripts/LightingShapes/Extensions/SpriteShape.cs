using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

namespace LightingShape {
		
	public class SpriteShape : Base {

		private Sprite originalSprite;
		private Sprite atlasSprite;

		private SpriteRenderer spriteRenderer;

		public override void ResetLocal() {
			base.ResetLocal();

			originalSprite = null;
			atlasSprite = null;
		}

		public SpriteRenderer GetSpriteRenderer() {
			if (spriteRenderer != null) {
				return(spriteRenderer);
			}
			
			if (gameObject == null) {
				return(spriteRenderer);
			}

			if (spriteRenderer == null) {
				spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			}
			
			return(spriteRenderer);
		}

		public Sprite GetOriginalSprite() {
            if (originalSprite == null) {
                GetSpriteRenderer();

                if (spriteRenderer != null) {
                    originalSprite = spriteRenderer.sprite;
                }
            }
			return(originalSprite);
		}

		public Sprite GetAtlasSprite() {
			return(atlasSprite);
		}

		public void SetAtlasSprite(Sprite sprite) {
			atlasSprite = sprite;
		}
	}
}