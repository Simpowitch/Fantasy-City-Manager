using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

[System.Serializable]
public class LightingRoomShape {
    public LightingRoom2D.RoomType type = LightingRoom2D.RoomType.Collider;

    public ColliderShape colliderShape = new ColliderShape();
	public SpriteShape spriteShape = new SpriteShape();

    public void SetTransform(Transform t) {
		colliderShape.SetTransform(t);
		spriteShape.SetTransform(t);
	}

    public void ResetLocal() {
        colliderShape.ResetLocal();

		spriteShape.ResetLocal();
	}

	public void ResetWorld() {
		colliderShape.ResetWorld();

		colliderShape.ResetWorld();
	}

    public List<MeshObject> GetMeshes() {
		switch(type) {
			case LightingRoom2D.RoomType.Collider:
				return(colliderShape.GetMeshes());

		}
	
		return(null);
	}

}
