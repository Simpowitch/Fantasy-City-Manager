using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DayLighting;
using LightingSettings;

[ExecuteInEditMode]
public class DayLightingCollider2D : MonoBehaviour {
	public enum ColliderType {None, SpriteCustomPhysicsShape, Collider, Sprite}; // Sprite == Sprite offset
	public enum MaskType {None, Sprite, BumpedSprite};

	public LightingLayer collisionDayLayer = LightingLayer.Layer1;
	public LightingLayer maskDayLayer = LightingLayer.Layer1;
	
	public DayLightingColliderShape shape = new DayLightingColliderShape();
	public DayLightingColliderTransform transform2D = new DayLightingColliderTransform();

	public DayNormalMapMode normalMapMode = new DayNormalMapMode();
	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public ShadowMesh shadowMesh = new ShadowMesh();
 
	public static List<DayLightingCollider2D> list = new List<DayLightingCollider2D>();

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		shape.Reset();
		shape.transform = transform;

		transform2D.Update(shape);

		Generate();
	}

	public void OnDisable() {
		list.Remove(this);

		shape.Reset();
	}

	public bool InAnyCamera() {
		LightingManager2D manager = LightingManager2D.Get();
		CameraSettings[] cameraSettings = manager.cameraSettings;

		for(int i = 0; i < cameraSettings.Length; i++) {
			Camera camera = manager.GetCamera(i);

			if (camera == null) {
				continue;
			}

			// if application is running
			//if (cameraSettings[i].cameraType == CameraSettings.CameraType.SceneView) {
			//	continue;
			////}

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
		transform2D.Update(shape);

		if (transform2D.moved == true) {
			Generate();
		}
    }

	public void Generate() {
		shadowMesh.Generate(transform, shape, shape.height);
	}
	
	public void Initialize() {
		shape.Reset();

		Generate();
	}
}
