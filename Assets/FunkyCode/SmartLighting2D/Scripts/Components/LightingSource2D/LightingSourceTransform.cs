using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSourceTransform {

	private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

	public Vector2 position = Vector2.zero;
	public float rotation = 0f;
	private float size = 0f;
	private float angle = 360;
	private float outerAngle = 15;

	private Color color = Color.white;

	private Sprite sprite;
	private bool flipX = false;
	private bool flipY = false;

	private float normalIntensity = 1;
	private float normalDepth = 1;

	public void ForceUpdate() {
		update = true;
	}

	public void Update(LightingSource2D source) {
		Transform transform = source.transform;

		Vector2 position2D = LightingPosition.Get(transform);

		float rotation2D = transform.rotation.eulerAngles.z;
		
		if (position != position2D) {
			position = position2D;

			update = true;
		}

		if (rotation != rotation2D) {
			rotation = rotation2D;

			update = true;
		}

		if (size != source.size) {
			size = source.size;

			update = true;
		}

		if (sprite != source.sprite) {
			sprite = source.sprite;

			update = true;
		}

		if (flipX != source.spriteFlipX) {
			flipX = source.spriteFlipX;

			update = true;
		}

		if (flipY != source.spriteFlipY) {
			flipY = source.spriteFlipY;

			update = true;
		}

		if (angle != source.angle) {
			angle = source.angle;

			update = true;
		}
		
		if (outerAngle != source.outerAngle) {
			outerAngle = source.outerAngle;

			update = true;
		}
		
		if (normalIntensity != source.bumpMap.intensity) {
			normalIntensity = source.bumpMap.intensity;

			update = true;
		}

		if (normalDepth != source.bumpMap.depth) {
			normalDepth = source.bumpMap.depth;

			update = true;
		}

		// No need to update for Color and Alpha
		if (color != source.color) {
			color = source.color;
		}
	}
}