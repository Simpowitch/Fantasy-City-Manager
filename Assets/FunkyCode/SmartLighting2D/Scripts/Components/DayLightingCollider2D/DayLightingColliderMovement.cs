using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLightingColliderTransform {
	public bool moved = false;

	public Vector2 position = Vector2.zero;
	public Vector2 scale = Vector3.zero;
	public float rotation = 0;
	
	private bool flipX = false;
	private bool flipY = false;

	private float height = 0;

	private float sunDirection = 0;
	private float sunSoftness = 1;
	private float sunHeight = 1;

	private DayLightingColliderShape shape;

	public void Reset() {
		position = Vector2.zero;
		rotation = 0;
		scale = Vector3.zero;
	}

	public void SetShape(DayLightingColliderShape shape) {
		this.shape = shape;
	}

	public void Update() {
		if (shape.transform == null) {
			return;
		}
		
		Transform transform = shape.transform;

		Vector2 scale2D = transform.lossyScale;
		Vector2 position2D = transform.position;
		float rotation2D = transform.rotation.eulerAngles.z;

		SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

		moved = false;

		if (position != position2D) {

			position = position2D;

			// does not update shadow
		}

		if (sunDirection != Lighting2D.dayLightingSettings.direction) {
			sunDirection = Lighting2D.dayLightingSettings.direction;

			moved = true;
		}

		if (sunHeight != Lighting2D.dayLightingSettings.height) {
			sunHeight = Lighting2D.dayLightingSettings.height;

			moved = true;
		}

		if (sunSoftness != Lighting2D.dayLightingSettings.softness.intensity) {
			sunSoftness = Lighting2D.dayLightingSettings.softness.intensity;

			moved = true;
		}
				
		if (scale != scale2D) {
			scale = scale2D;

			moved = true;
		}

		if (rotation != rotation2D) {
			rotation = rotation2D;

			moved = true;
		}

		if (height != shape.height) {
			height = shape.height;

			moved = true;
		}

		// Unnecesary check
		if (shape.height < 0.01f) {
			shape.height = 0.01f;
		}

		if (shape.colliderType == DayLightingCollider2D.ColliderType.SpriteCustomPhysicsShape) {
			if (spriteRenderer != null) {
				if (spriteRenderer.flipX != flipX || spriteRenderer.flipY != flipY) {
					flipX = spriteRenderer.flipX;
					flipY = spriteRenderer.flipY;

					moved = true;
					
					shape.ResetLocal(); // World
				}
				
				/* Sprite frame change
				if (shape.GetOriginalSprite() != spriteRenderer.sprite) {
					shape.SetOriginalSprite(spriteRenderer.sprite);
					shape.SetAtlasSprite(null); // Only For Sprite Mask?

					moved = true;
					
					shape.Reset(); // Local
				}
				*/
			}
		}

		/* Sprite Frame Change
		if (shape.maskType == LightingCollider2D.MaskType.Sprite) {
			if (spriteRenderer != null && shape.GetOriginalSprite() != spriteRenderer.sprite) {
				shape.SetOriginalSprite(spriteRenderer.sprite);
				shape.SetAtlasSprite(null);

				moved = true;
			}
		}
		/*/
	}
}
