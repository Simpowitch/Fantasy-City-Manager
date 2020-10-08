using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LightingSettings;
using EventHandling;

public enum MaskEffect {Lit, Unlit}

[ExecuteInEditMode]
public class LightingCollider2D : MonoBehaviour {
	public enum ColliderType {None, SpriteCustomPhysicsShape, Collider, Mesh, SkinnedMesh};
	public enum MaskType {None, Sprite, BumpedSprite,  SpriteCustomPhysicsShape, Collider, Mesh, SkinnedMesh};

	public LightingLayer lightingCollisionLayer = LightingLayer.Layer1;
	public LightingLayer lightingMaskLayer = LightingLayer.Layer1;
	public MaskEffect maskEffect = MaskEffect.Lit;
	
	public LightingColliderShape shape = new LightingColliderShape();
	public LightingColliderTransform transform2D = new LightingColliderTransform();

	public NormalMapMode normalMapMode = new NormalMapMode();
	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public event CollisionEvent2D collisionEvents;

	public static List<LightingCollider2D> list = new List<LightingCollider2D>();

	public void AddEvent(CollisionEvent2D collisionEvent) {
		collisionEvents += collisionEvent;
	}

	public void CollisionEvent(LightCollision2D collision) {
		if (collisionEvents != null) {
			collisionEvents (collision);
		}
	}

	// 1.5f??
	public bool InLightSource(LightingBuffer2D buffer) {
		if (Vector2.Distance(transform2D.position, buffer.lightSource.transform2D.position) > shape.GetRadiusWorld() + buffer.lightSource.size * 1.5f) {
			return(false);
		}

		return(true);
	}

	// In Any Camera?
	public bool InCamera() { 
		float cameraSize = GetComponent<Camera>().orthographicSize;
		
		float distance = Vector2.Distance(transform.position, GetComponent<Camera>().transform.position);
		float size = Mathf.Sqrt((cameraSize * 2f) * (cameraSize * 2f)) + shape.GetRadiusWorld();
		
        return (distance < size);
    }

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		shape.SetGameObject(gameObject);

		transform2D.Update(shape);

		Initialize();

		UpdateNearbyLights();
	}

	public void OnDisable() {
		list.Remove(this);

		UpdateNearbyLights();
	}

	public void UpdateNearbyLights() {
		float distance = shape.GetRadiusWorld();
		foreach (LightingSource2D id in LightingSource2D.GetList()) {
			bool draw = DrawOrNot(id);

			if (draw == false) {
				continue;
			}
			
			if (Vector2.Distance (id.transform.position, transform.position) < distance + id.size) {
				id.ForceUpdate();
			}
		}
	}

	static public List<LightingCollider2D> GetList() {
		return(list);
	}

	public void Initialize() {
		transform2D.Reset();

		transform2D.UpdateNeeded = true;

		shape.ResetLocal();
	}

	public bool DrawOrNot(LightingSource2D id) {
		if (id.layerSetting == null) {
			return(false);
		}

		for(int i = 0; i < id.layerSetting.Length; i++) {
			if (id.layerSetting[i] == null) {
				continue;
			}

			int layerID = (int)id.layerSetting[i].layerID;
			
			switch(id.layerSetting[i].type) {
				case LightingLayerType.ShadowAndMask:
					if (layerID == (int)lightingCollisionLayer || layerID == (int)lightingMaskLayer) {
						return(true);
					}
				break;

				case LightingLayerType.MaskOnly:
					if (layerID == (int)lightingMaskLayer) {
						return(true);
					}
				break;

				case LightingLayerType.ShadowOnly:
					if (layerID  == (int)lightingCollisionLayer) {
						return(true);
					}
				break;
			}
		}

		return(false);
	}

	public void UpdateLoop() {
		transform2D.Update(shape);

		if (transform2D.UpdateNeeded) {
			shape.ResetWorld();

			UpdateNearbyLights();
		}
	}
}