using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DayLighting;
using LightingShape;

[System.Serializable]
public class DayLightingColliderShape {
	public DayLightingCollider2D.MaskType maskType = DayLightingCollider2D.MaskType.Sprite;
    public DayLightingCollider2D.ColliderType colliderType = DayLightingCollider2D.ColliderType.SpriteCustomPhysicsShape;
    
	public Transform transform;
   
    public DayLightingColliderTransform transform2D = new DayLightingColliderTransform();

    public SpriteShape spriteShape = new SpriteShape();
    public SpriteCustomPhysicsShape spriteCustomPhysicsShape = new SpriteCustomPhysicsShape();
	public ColliderShape colliderShape = new ColliderShape();

    public float height = 1;
    public ShadowMesh shadowMesh = new ShadowMesh();

    public void SetTransform(Transform t) {
        transform = t;

        transform2D.SetShape(this);

        spriteShape.SetTransform(t);
        spriteCustomPhysicsShape.SetTransform(t);
		
		colliderShape.SetTransform(t);
    }

    public void ResetLocal() {
		spriteShape.ResetLocal();
		spriteCustomPhysicsShape.ResetLocal();

		colliderShape.ResetLocal();
	}

    public void ResetWorld() {
		spriteCustomPhysicsShape.ResetWorld();

		colliderShape.ResetWorld();
	}

	public List<Polygon2D> GetPolygonsLocal() {
		switch(colliderType) {
			case DayLightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsLocal());

			case DayLightingCollider2D.ColliderType.Collider:
				return(colliderShape.GetPolygonsLocal());

		}

		return(null);
	}

    public List<Polygon2D> GetPolygonsWorld() {
		switch(colliderType) {
			case DayLightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsWorld());

			case DayLightingCollider2D.ColliderType.Collider:
				return(colliderShape.GetPolygonsWorld());
		}

		return(null);
	}
}
