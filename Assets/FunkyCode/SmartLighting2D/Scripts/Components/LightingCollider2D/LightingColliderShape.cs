using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

[System.Serializable]
public class LightingColliderShape {
	public LightingCollider2D.ColliderType colliderType = LightingCollider2D.ColliderType.SpriteCustomPhysicsShape;
	public LightingCollider2D.MaskType maskType = LightingCollider2D.MaskType.Sprite;
	public float shadowDistance = 0;

	public ColliderShape colliderShape = new ColliderShape();
	public CompositeShape compositeShape = new CompositeShape();

	public SpriteShape spriteShape = new SpriteShape();
	public SpriteCustomPhysicsShape spriteCustomPhysicsShape = new SpriteCustomPhysicsShape();
	
	public MeshShape meshShape = new MeshShape();
	public SkinnedMeshShape skinnedMeshShape = new SkinnedMeshShape();
	
	public LightingColliderTransform transform2D = new LightingColliderTransform();
	public Transform transform;
	
	public void SetTransform(Transform setTransform) {
		transform = setTransform;

		transform2D.SetShape(this);

		spriteShape.SetTransform(transform);
		spriteCustomPhysicsShape.SetTransform(transform);

		colliderShape.SetTransform(transform);
		compositeShape.SetTransform(transform);

		meshShape.SetTransform(transform);
		skinnedMeshShape.SetTransform(transform);
	}

	public void ResetLocal() {
		spriteShape.ResetLocal();
		spriteCustomPhysicsShape.ResetLocal();

		colliderShape.ResetLocal();
		compositeShape.ResetLocal();

		meshShape.ResetLocal(); 
		skinnedMeshShape.ResetLocal(); 

		ResetWorld();
	}

	public void ResetWorld() {
		spriteShape.ResetWorld();
		spriteCustomPhysicsShape.ResetWorld();

		colliderShape.ResetWorld();
		compositeShape.ResetWorld();

		meshShape.ResetWorld();
		skinnedMeshShape.ResetWorld();
	}

	public bool IsEdgeCollider() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.Collider2D:
				return(colliderShape.edgeCollider2D);
		}
		
		return(false);
	}

	public float GetRadiusWorld() {
		float sx = Mathf.Abs(transform.lossyScale.x);
		float sy = Mathf.Abs(transform.lossyScale.y);

		float multiplier = Mathf.Max(sx, sy);

		switch(colliderType) {
			case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetRadius() * multiplier);
				
			case LightingCollider2D.ColliderType.Collider2D:
				return(colliderShape.GetRadius() * multiplier);

			case LightingCollider2D.ColliderType.CompositeCollider2D:
				return(compositeShape.GetRadius() * multiplier);

			// case LightingCollider2D.ColliderType.Mesh:
            // case LightingCollider2D.ColliderType.SkinnedMesh:
		}

		switch (maskType) {
			case LightingCollider2D.MaskType.BumpedSprite:
            case LightingCollider2D.MaskType.Sprite:

				SpriteRenderer spriteRenderer = spriteShape.GetSpriteRenderer();
				if (spriteRenderer != null && spriteRenderer.drawMode != SpriteDrawMode.Simple) {
					return(1000f);
				};

				// return (spriteShape.GetFrustumDistance() * multiplier);

				return(1000f);

			case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                return (spriteCustomPhysicsShape.GetRadius() * multiplier);

			case LightingCollider2D.MaskType.Collider2D:
				return(colliderShape.GetRadius() * multiplier);

			//case LightingCollider2D.MaskType.CompositeCollider:
			//	return(compositeShape.GetRadius() * multiplier);
        }

		return(1000f);
	}
	
	public List<MeshObject> GetMeshes() {
		switch(maskType) {
			case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetMeshes());

			case LightingCollider2D.MaskType.Collider2D:
				return(colliderShape.GetMeshes());
		
			case LightingCollider2D.MaskType.CompositeCollider2D:
				return(compositeShape.GetMeshes());
				
			case LightingCollider2D.MaskType.MeshRenderer:
				return(meshShape.GetMeshes());

			case LightingCollider2D.MaskType.SkinnedMeshRenderer:
				return(skinnedMeshShape.GetMeshes());

		}
	
		return(null);
	}

	public List<Polygon2D> GetPolygonsLocal() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsLocal());

			case LightingCollider2D.ColliderType.Collider2D:
				return(colliderShape.GetPolygonsLocal());

			case LightingCollider2D.ColliderType.CompositeCollider2D:
				return(compositeShape.GetPolygonsLocal());

			case LightingCollider2D.ColliderType.MeshRenderer:
				return(null);

			case LightingCollider2D.ColliderType.SkinnedMeshRenderer:
				return(null);
		}

		return(null);
	}

	public List<Polygon2D> GetPolygonsWorld() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsWorld());

			case LightingCollider2D.ColliderType.Collider2D:
				return(colliderShape.GetPolygonsWorld());

			case LightingCollider2D.ColliderType.CompositeCollider2D:
				return(compositeShape.GetPolygonsWorld());

			case LightingCollider2D.ColliderType.MeshRenderer:
				return(meshShape.GetPolygonsWorld());
				
			case LightingCollider2D.ColliderType.SkinnedMeshRenderer:
				return(skinnedMeshShape.GetPolygonsWorld());
		}

		return(null);
	}
}