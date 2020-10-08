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

		// pointsColliding = new List<Vector2D

		lightingEventState = LightingEventState.None;
		
	}
}

#if UNITY_2017_4_OR_NEWER

	public class LightTilemapCollision2D {
		public enum CollisionType {Tilemap}

		public LightingSource2D lightSource;

		public LightingTilemapCollider2D tilemapCollider;

		public List<Vector2D> pointsColliding;
		public LightingEventState lightingEventState;
	}

#endif