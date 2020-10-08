using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventHandling {
    public class Tilemap
    {
        
        public void UpdateTilemapEventHandling() {
            /*
            List<LightTilemapCollision2D> collisions = EventHandling_GetCollisions();
            EventHandling_RemoveHiddenCollisions(collisions);

            if (collisions.Count < 1) {
                for(int i = 0; i < lightignEventCache.Count; i++) {
                    LightingCollider2D collider = lightignEventCache[i];
                    
                    LightCollision2D collision = new LightCollision2D();
                    collision.lightSource = this;
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
                    collision.lightSource = this;
                    collision.collider = collider;
                    collision.pointsColliding = null;
                    collision.lightingEventState = LightingEventState.OnCollisionExit;

                    collider.CollisionEvent(collision);
                    
                    lightignEventCache.Remove(collider);
                }
            }
            
            foreach(LightCollision2D collision in collisions) {
                if (lightignEventCache.Contains(collision.collider)) {
                    collision.lightingEventState = LightingEventState.OnCollision;
                } else {
                    collision.lightingEventState = LightingEventState.OnCollisionEnter;
                    lightignEventCache.Add(collision.collider);
                }
            
                collision.collider.CollisionEvent(collision);
            }*/
        }

    }
}
