using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingColliderTransform {

	private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

	public Vector2 position = Vector2.zero;
	public Vector2 scale = Vector3.zero;
	public float rotation = 0;
	public float shadowHeight = 0;

	private Vector3 position3D = Vector3.zero;
	private bool flipX = false;
	private bool flipY = false;
	private Vector2 size = Vector2.one;

	LightingColliderShape shape;

	public void SetShape(LightingColliderShape shape) {
		this.shape = shape;
	}

	public void Reset() {
		position = Vector2.zero;
		rotation = 0;
		scale = Vector3.zero;
	}

	public void Update() {
		if (shape.transform == null) {
			return;
		}

		Transform transform = shape.transform;

		Vector3 newPosition3D = transform.position;
		Vector2 position2D = LightingPosition.Get(transform);

		Vector2 scale2D = transform.lossyScale;
		float rotation2D = transform.eulerAngles.z;

		update = false;

		if (position3D != newPosition3D) {
			position3D = newPosition3D;

			update = true;
		}

		if (position != position2D) {
			position = position2D;

			update = true;
		}
				
		if (scale != scale2D) {
			scale = scale2D;

			update = true;
		}

		if (rotation != rotation2D) {
			rotation = rotation2D;

			update = true;
		}

		if (shape.maskType == LightingCollider2D.MaskType.SpriteCustomPhysicsShape || shape.colliderType == LightingCollider2D.ColliderType.SpriteCustomPhysicsShape) {
			SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

			if (spriteRenderer != null) {
				if (spriteRenderer.flipX != flipX || spriteRenderer.flipY != flipY) {
					flipX = spriteRenderer.flipX;
					flipY = spriteRenderer.flipY;

					shape.ResetWorld();

					update = true;
				}
				
				if (shape.spriteShape.GetOriginalSprite() != spriteRenderer.sprite) {
					shape.ResetLocal();

					update = true;
				}
			}
		}

		if (shape.maskType == LightingCollider2D.MaskType.Sprite || shape.maskType == LightingCollider2D.MaskType.BumpedSprite) {
			SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

			if (spriteRenderer != null) {
				if (spriteRenderer.size != size) {
					size = spriteRenderer.size;
					
					update = true;
				}

				if (shape.spriteShape.GetOriginalSprite() != spriteRenderer.sprite) {
					shape.ResetLocal();

					update = true;
				}
			}
		}
	}
}