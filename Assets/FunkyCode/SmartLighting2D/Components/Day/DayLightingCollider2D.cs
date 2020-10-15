using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DayLighting;
using LightingSettings;

[ExecuteInEditMode]
public class DayLightingCollider2D : MonoBehaviour {
	public enum ColliderType {None, SpriteCustomPhysicsShape, Collider, Sprite}; 
	public enum MaskType {None, Sprite, BumpedSprite};

	public LightingLayer collisionDayLayer = LightingLayer.Layer1;
	public LightingLayer maskDayLayer = LightingLayer.Layer1;
	
	public DayLightingColliderShape mainShape = new DayLightingColliderShape();
	public List<DayLightingColliderShape> shapes = new List<DayLightingColliderShape>();

	public DayNormalMapMode normalMapMode = new DayNormalMapMode();
	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public bool applyToChildren = false;

	public static List<DayLightingCollider2D> list = new List<DayLightingCollider2D>();

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		Initialize();
	}

	public void OnDisable() {
		list.Remove(this);
	}

	public bool InAnyCamera() {
		LightingManager2D manager = LightingManager2D.Get();
		CameraSettings[] cameraSettings = manager.cameraSettings;

		for(int i = 0; i < cameraSettings.Length; i++) {
			Camera camera = manager.GetCamera(i);

			if (camera == null) {
				continue;
			}

			float dist = Vector2.Distance(transform.position, camera.transform.position);
			float cameraSize = camera.orthographicSize;
			float cameraSize2 = (cameraSize * 2f);
			float diameter = Mathf.Sqrt(cameraSize2 * cameraSize2) + 5; // 5 = Size

			if (dist < diameter) {
				return(true);
			}
		}

		return(false);
	}

	static public List<DayLightingCollider2D> GetList() {
		return(list);
	}

    public void UpdateLoop() {
		foreach(DayLightingColliderShape shape in shapes) {
			shape.height = mainShape.height;
			
			shape.transform2D.Update();

			if (shape.transform2D.moved) {
				shape.shadowMesh.Generate(shape);
			}
		}	
    }

	public void Initialize() {
		shapes.Clear();

		mainShape.SetTransform(transform);
		mainShape.ResetLocal();

		mainShape.transform2D.Update();
		
		shapes.Add(mainShape);

		if (applyToChildren) {
			foreach(Transform childTransform in transform) {

				DayLightingColliderShape shape = new DayLightingColliderShape();
				shape.maskType = mainShape.maskType;
				shape.colliderType = mainShape.colliderType;
				shape.height = mainShape.height;

				shape.SetTransform(childTransform);
				shape.ResetLocal();

				shape.transform2D.Update();
		
				shapes.Add(shape);

			}
		}

		foreach(DayLightingColliderShape shape in shapes) {
			shape.shadowMesh.Generate(shape);
		}
	}

	void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.sceneView.drawGizmos == false) {
			return;
		}
		
		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		
		if (mainShape.colliderType != DayLightingCollider2D.ColliderType.None) {
			foreach(DayLightingColliderShape shape in shapes) {
				shape.ResetWorld();
			
				List<Polygon2D> poly = shape.GetPolygonsWorld();

				if (poly != null) {
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
}
