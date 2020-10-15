using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LightCollision2D {

	public LightingSource2D lightSource;
	public LightingCollider2D collider;

	public List<Vector2> pointsColliding;
	public LightingEventState lightingEventState;

	public LightCollision2D(bool _active) {
		lightSource = null;

		collider = null;
		
		pointsColliding = null;

		lightingEventState = LightingEventState.None;
	}
}