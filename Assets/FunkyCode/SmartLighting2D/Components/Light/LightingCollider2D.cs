using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LightingSettings;
using EventHandling;

public enum MaskEffect {Lit, Unlit}

public class LayerManager<T> {
	public List<T>[] layerList;

	public LayerManager() {
		layerList = new List<T>[10];

		for(int i = 0; i < 10; i++) {
			layerList[i] = new List<T>();
		}
	}

	public int Update(int targetLayer, int newLayer, T obj) {
		if (targetLayer != newLayer) {
			if (targetLayer > -1) {
				layerList[targetLayer].Remove(obj);
			}

			targetLayer = newLayer;

			layerList[targetLayer].Add(obj);
		}

		return(targetLayer);
	}

	public void Remove(int targetLayer, T obj) {
		if (targetLayer > -1) {
			layerList[targetLayer].Remove(obj);
		}
	}
}

[ExecuteInEditMode]
public class LightingCollider2D : MonoBehaviour {
	public enum ColliderType {None, SpriteCustomPhysicsShape, Collider2D, CompositeCollider2D, MeshRenderer, SkinnedMeshRenderer};
	public enum MaskType {None, Sprite, BumpedSprite,  SpriteCustomPhysicsShape, Collider2D, CompositeCollider2D, MeshRenderer, SkinnedMeshRenderer};

	public LightingLayer lightingCollisionLayer = LightingLayer.Layer1;
	public LightingLayer shadowEffectLayer = LightingLayer.Layer1;
	public LightingLayer lightingMaskLayer = LightingLayer.Layer1;

	// Mask Effect
	public MaskEffect maskEffect = MaskEffect.Lit;
	
	public LightingColliderShape mainShape = new LightingColliderShape();
	public List<LightingColliderShape> shapes = new List<LightingColliderShape>();

	public NormalMapMode normalMapMode = new NormalMapMode();
	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public bool applyToChildren = false;

	public event CollisionEvent2D collisionEvents;
	// List Manager 
	public static List<LightingCollider2D> list = new List<LightingCollider2D>();

	public static LayerManager<LightingCollider2D> layerManagerMask = new LayerManager<LightingCollider2D>();
	public static LayerManager<LightingCollider2D> layerManagerCollision = new LayerManager<LightingCollider2D>();
	public static LayerManager<LightingCollider2D> layerManagerEffect = new LayerManager<LightingCollider2D>();
	private int listMaskLayer = -1;
	private int listCollisionLayer = -1;
	private int listEffectLayer = -1;
	
	public void OnEnable() {
		list.Add(this);
		UpdateLayerList();

		LightingManager2D.Get();

		Initialize();

		UpdateNearbyLights();
	}

	public void OnDisable() {
		list.Remove(this);
		ClearLayerList();
		
		UpdateNearbyLights();
	}

	public void Update() {
		UpdateLayerList();
	}












	// Layer List
	void ClearLayerList() {
		layerManagerMask.Remove(listMaskLayer, this);
		layerManagerCollision.Remove(listCollisionLayer, this);
		layerManagerEffect.Remove(listEffectLayer, this);

		listMaskLayer = -1;
		listCollisionLayer = -1;
		listEffectLayer = -1;
	}

	void UpdateLayerList() {
		listMaskLayer = layerManagerMask.Update(listMaskLayer, (int)lightingMaskLayer, this);
		listCollisionLayer = layerManagerCollision.Update(listCollisionLayer, (int)lightingCollisionLayer, this);
		listEffectLayer = layerManagerEffect.Update(listEffectLayer, (int)shadowEffectLayer, this);
	}

	static public List<LightingCollider2D> GetMaskList(int layer) {
		return(layerManagerMask.layerList[layer]);
	}

	static public List<LightingCollider2D> GetCollisionList(int layer) {
		return(layerManagerCollision.layerList[layer]);
	}
		static public List<LightingCollider2D> GetEffectList(int layer) {
		return(layerManagerEffect.layerList[layer]);
	}
	
	static public List<LightingCollider2D> GetList() {
		return(list);
	}
	










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
		if (Vector2.Distance(mainShape.transform2D.position, buffer.lightSource.transform2D.position) > mainShape.GetRadiusWorld() + buffer.lightSource.size * 1.5f) {
			return(false);
		}

		return(true);
	}

	// In Any Camera?
	public bool InCamera() { 
		float cameraSize = GetComponent<Camera>().orthographicSize;
		
		float distance = Vector2.Distance(transform.position, GetComponent<Camera>().transform.position);
		float size = Mathf.Sqrt((cameraSize * 2f) * (cameraSize * 2f)) + mainShape.GetRadiusWorld();
		
        return (distance < size);
    }

	public void UpdateNearbyLights() {
		float distance = mainShape.GetRadiusWorld();
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

	 private void AddChildShapes(Transform parent) {
        foreach (Transform child in parent) {
			LightingColliderShape shape = new LightingColliderShape();
			shape.maskType = mainShape.maskType;
			shape.colliderType = mainShape.colliderType;
			shape.shadowDistance = mainShape.shadowDistance;
			
			shape.SetTransform(child);
			shape.transform2D.Update();
			
			shapes.Add(shape);

			AddChildShapes(child);
        }
    }


	public void Initialize() {
		shapes.Clear();

		mainShape.SetTransform(transform);
		mainShape.transform2D.Reset();
		mainShape.transform2D.Update();
		mainShape.transform2D.UpdateNeeded = true;

		shapes.Add(mainShape);

		if (applyToChildren) {
			AddChildShapes(transform);
		}

		foreach(LightingColliderShape shape in shapes) {
			shape.ResetLocal();
		}
	}

	public bool DrawOrNot(LightingSource2D id) {
		LayerSetting[] layerSetting = id.GetLayerSettings();

		if (layerSetting == null) {
			return(false);
		}

		for(int i = 0; i < layerSetting.Length; i++) {
			if (layerSetting[i] == null) {
				continue;
			}

			int layerID = (int)layerSetting[i].layerID;
			
			switch(layerSetting[i].type) {
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
		bool updateLights = false;

		foreach(LightingColliderShape shape in shapes) {
			shape.transform2D.Update();

			if (shape.transform2D.UpdateNeeded) {
				shape.ResetWorld();

				updateLights = true;
			}
		}

		if (updateLights) {
			UpdateNearbyLights();
		}

	}

	void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.sceneView.drawGizmos == false) {
			return;
		}

		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		
		if (mainShape.colliderType != LightingCollider2D.ColliderType.None) {
			foreach(LightingColliderShape shape in shapes) {
				List<Polygon2D> poly = shape.GetPolygonsWorld();
				
				foreach(Polygon2D polygon in poly) {
					for(int i = 0; i < polygon.pointsList.Count; i++) {
						Vector2D p0 = polygon.pointsList[i];
						Vector2D p1 = polygon.pointsList[(i + 1) % polygon.pointsList.Count];

						Vector3 a = new Vector3((float)p0.x, (float)p0.y, transform.position.z);
						Vector3 b = new Vector3((float)p1.x, (float)p1.y, transform.position.z);

						Gizmos.DrawLine(a, b);
					}
				}
			}
		}
    }
}