using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventHandling {

    public class Object {
        public List<LightingCollider2D> lightignEventCache = new List<LightingCollider2D>();
		public List<LightCollision2D> collisions = new List<LightCollision2D>();

		public void Update(LightingSource2D lightingSource, bool useColliders, bool useTilemapColliders) {
			if (lightingSource == null) {
				return;
			}
			
			collisions.Clear();

			EventHandling.Collider.GetCollisions(collisions, lightingSource);
			
			if (useColliders) {
				EventHandling.Collider.RemoveHiddenCollisions(collisions, lightingSource);
			}

			if (useTilemapColliders) {
				EventHandling.Tilemap2D.RemoveHiddenCollisions(collisions, lightingSource);
			}
			
			

			if (collisions.Count < 1) {
				for(int i = 0; i < lightignEventCache.Count; i++) {
					LightingCollider2D collider = lightignEventCache[i];
					
					LightCollision2D collision = new LightCollision2D();
					collision.lightSource = lightingSource;
					collision.collider = collider;
					collision.pointsColliding = null;
					collision.lightingEventState = LightingEventState.OnCollisionExit;

					collider.CollisionEvent(collision);
				}

				lightignEventCache.Clear();

				return;
			}
				
			List<LightingCollider2D> eventColliders = new List<LightingCollider2D>();

			foreach(LightCollision2D collision in collisions) {
				eventColliders.Add(collision.collider);
			}

			for(int i = 0; i < lightignEventCache.Count; i++) {
				LightingCollider2D collider = lightignEventCache[i];
				if (eventColliders.Contains(collider) == false) {
					
					LightCollision2D collision = new LightCollision2D();
					collision.lightSource = lightingSource;
					collision.collider = collider;
					collision.pointsColliding = null;
					collision.lightingEventState = LightingEventState.OnCollisionExit;

					collider.CollisionEvent(collision);
					
					lightignEventCache.Remove(collider);
				}
			}
			
			for(int i = 0; i < collisions.Count; i++) {
				LightCollision2D collision = collisions[i];
				
				if (lightignEventCache.Contains(collision.collider)) {
					collision.lightingEventState = LightingEventState.OnCollision;
				} else {
					collision.lightingEventState = LightingEventState.OnCollisionEnter;
					lightignEventCache.Add(collision.collider);
				}
			
				collision.collider.CollisionEvent(collision);
			}
		}
	}
}