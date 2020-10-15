using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

[System.Serializable]
public class LightingOcclusionShape {
    public LightingOcclusion2D.ShadowType shadowType = LightingOcclusion2D.ShadowType.Collider;

	public ColliderShape colliderShape = new ColliderShape();
    public SpriteCustomPhysicsShape spriteCustomPhysicsShape = new SpriteCustomPhysicsShape();

	public Transform transform;
	
	public void SetTransform(Transform t) {
		transform = t.transform;

		colliderShape.SetTransform(t);

        spriteCustomPhysicsShape.SetTransform(t);
	}

	public void ResetLocal() {
		colliderShape.ResetLocal();

        spriteCustomPhysicsShape.ResetLocal();

		ResetWorld();
	}

	public void ResetWorld() {
		colliderShape.ResetWorld();

        spriteCustomPhysicsShape.ResetWorld();
	}

	public bool IsEdgeCollider() {
		switch(shadowType) {
			case LightingOcclusion2D.ShadowType.Collider:
				return(colliderShape.edgeCollider2D);
		}
		
		return(false);
	}

	public List<MeshObject> GetMeshes() {
		switch(shadowType) {
			case LightingOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetMeshes());

           case LightingOcclusion2D.ShadowType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetMeshes());
		}
	
		return(null);
	}

	public List<Polygon2D> GetPolygonsLocal() {
		switch(shadowType) {
			case LightingOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsLocal());

            case LightingOcclusion2D.ShadowType.SpriteCustomPhysicsShape:
                return(spriteCustomPhysicsShape.GetPolygonsLocal());
		}
		return(null);
	}

	public List<Polygon2D> GetPolygonsWorld() {
		switch(shadowType) {
			case LightingOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsWorld());

            case LightingOcclusion2D.ShadowType.SpriteCustomPhysicsShape:
                return(spriteCustomPhysicsShape.GetPolygonsWorld());
		}
		return(null);
	}
}