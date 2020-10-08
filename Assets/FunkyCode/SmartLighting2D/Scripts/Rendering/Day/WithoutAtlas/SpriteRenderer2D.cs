using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day.WithoutAtlas {

    public class SpriteRenderer2D {

		static public void Draw(Camera camera, Vector2 offset, float z, int layer) {
            List<DayLightingCollider2D> colliderList = DayLightingCollider2D.GetList();
			int colliderCount = colliderList.Count;

			Vector2 objectOffset;	

			float dayLightRotation = (Lighting2D.dayLightingSettings.direction - 180) * Mathf.Deg2Rad;
			float dayLightHeight = Lighting2D.dayLightingSettings.bumpMap.height;
			float dayLightStrength = Lighting2D.dayLightingSettings.bumpMap.strength;

			for(int i = 0; i < colliderCount; i++) {
				DayLightingCollider2D id = colliderList[i];

				if ((int)id.maskDayLayer != layer) {
					continue;
				}
			
				if (id.InAnyCamera() == false) {
					continue;
				}

				switch(id.shape.maskType) {
					case DayLightingCollider2D.MaskType.None:
						continue;

					case DayLightingCollider2D.MaskType.Sprite:
						Material material = Lighting2D.materials.GetWhiteSprite();

						UnityEngine.SpriteRenderer spriteRenderer = id.shape.GetSpriteRenderer();

						if (spriteRenderer == null || spriteRenderer.sprite == null) {
							break;
						}

						objectOffset.x = id.transform2D.position.x + offset.x;
						objectOffset.y = id.transform2D.position.y + offset.y;

						material.mainTexture = spriteRenderer.sprite.texture;

						Universal.WithoutAtlas.Sprite.Draw(id.spriteMeshObject, material, spriteRenderer, objectOffset, id.transform2D.scale, id.transform2D.rotation, z);
					
					break;

					case DayLightingCollider2D.MaskType.BumpedSprite:

						spriteRenderer = id.shape.GetSpriteRenderer();

						if (spriteRenderer == null || spriteRenderer.sprite == null) {
							break;
						}

						Texture bumpTexture = id.normalMapMode.GetBumpTexture();

						if (bumpTexture == null) {
							break;
						}

						material = Lighting2D.materials.GetBumpedDaySprite();
	
						objectOffset.x = id.transform2D.position.x + offset.x;
						objectOffset.y = id.transform2D.position.y + offset.y;


						float rotation = dayLightRotation - id.transform.eulerAngles.z * Mathf.Deg2Rad;
					

						material.SetFloat("_LightRX", Mathf.Cos(rotation) * dayLightStrength);
						material.SetFloat("_LightRY", Mathf.Sin(rotation) * dayLightStrength);
						material.SetFloat("_LightRZ", -dayLightHeight);
						material.SetTexture("_Bump", bumpTexture);

						material.mainTexture = spriteRenderer.sprite.texture;

						Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, objectOffset, id.transform.lossyScale, id.transform.rotation.eulerAngles.z, z);

					break;
				}
			}
		}
	}
}