using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

[System.Serializable]
public class LightingColliderShape {
	public LightingCollider2D.ColliderType colliderType = LightingCollider2D.ColliderType.SpriteCustomPhysicsShape;
	public LightingCollider2D.MaskType maskType = LightingCollider2D.MaskType.Sprite;

	public ColliderShape colliderShape = new ColliderShape();
	
	public SpriteShape spriteShape = new SpriteShape();
	public SpriteCustomPhysicsShape spriteCustomPhysicsShape = new SpriteCustomPhysicsShape();
	
	public MeshShape meshShape = new MeshShape();
	public SkinnedMeshShape skinnedMeshShape = new SkinnedMeshShape();

	public GameObject gameObject;
	public Transform transform;
	
	public void SetGameObject(GameObject g) {
		gameObject = g;
		transform = g.transform;

		colliderShape.SetGameObject(g);

		spriteShape.SetGameObject(g);
		spriteCustomPhysicsShape.SetGameObject(g);

		meshShape.SetGameObject(g);
		skinnedMeshShape.SetGameObject(g);
	}

	public void ResetLocal() {
		colliderShape.ResetLocal();

		spriteShape.ResetLocal();
		spriteCustomPhysicsShape.ResetLocal();

		meshShape.ResetLocal(); 
		skinnedMeshShape.ResetLocal(); 

		ResetWorld();
	}

	public void ResetWorld() {
		colliderShape.ResetWorld();

		spriteShape.ResetWorld();
		spriteCustomPhysicsShape.ResetWorld();

		meshShape.ResetWorld();
		skinnedMeshShape.ResetWorld();
	}

	public bool IsEdgeCollider() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.Collider:
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
			case LightingCollider2D.ColliderType.Collider:
				return(colliderShape.GetRadius() * multiplier);

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

			case LightingCollider2D.MaskType.Collider:
				return(colliderShape.GetRadius() * multiplier);
        }

		return(1000f);
	}
	
	public List<MeshObject> GetMeshes() {
		switch(maskType) {
			case LightingCollider2D.MaskType.Collider:
				return(colliderShape.GetMeshes());

			case LightingCollider2D.MaskType.Mesh:
				return(meshShape.GetMeshes());

			case LightingCollider2D.MaskType.SkinnedMesh:
				return(skinnedMeshShape.GetMeshes());

			case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetMeshes());
		}
	
		return(null);
	}

	public List<Polygon2D> GetPolygonsLocal() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.Collider:
				return(colliderShape.GetPolygonsLocal());

			case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsLocal());

			case LightingCollider2D.ColliderType.Mesh:
				return(null);

			case LightingCollider2D.ColliderType.SkinnedMesh:
				return(null);
		}

		return(null);
	}

	public List<Polygon2D> GetPolygonsWorld() {
		switch(colliderType) {
			case LightingCollider2D.ColliderType.Collider:
				return(colliderShape.GetPolygonsWorld());

			case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
				return(spriteCustomPhysicsShape.GetPolygonsWorld());

			case LightingCollider2D.ColliderType.Mesh:
				return(meshShape.GetPolygonsWorld());
				
			case LightingCollider2D.ColliderType.SkinnedMesh:
				return(skinnedMeshShape.GetPolygonsWorld());
		}

		return(null);
	}
}