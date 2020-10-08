using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#if UNITY_2017_4_OR_NEWER
using UnityEngine.Tilemaps;
#endif

public class EditorGameObjects : MonoBehaviour
{	static public Camera GetCamera() {
		Camera camera = null;

		if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null) {
			camera = SceneView.lastActiveSceneView.camera;
		} else if (Camera.main != null) {
			camera = Camera.main;
		}
		return(camera);
	}

	static public Vector3 GetCameraPoint() {
		Vector3 pos = Vector3.zero;

		Camera camera = GetCamera();
		if (camera != null) {
			Ray worldRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
			pos = worldRay.origin;
			pos.z = 0;
		} else {
			Debug.LogError("Scene Camera Not Found");
		}

		return(pos);
	}

	[MenuItem("GameObject/2D Light/Light Source", false, 4)]
    static void CreateLightSource() {	
		GameObject newGameObject = new GameObject("2D Light Source");

		newGameObject.AddComponent<LightingSource2D>();

		newGameObject.transform.position = GetCameraPoint();
	}

	[MenuItem("GameObject/2D Light/Light Collider", false, 4)]
    static void CreateLightCollider() {
		GameObject newGameObject = new GameObject("2D Light Collider");

		newGameObject.AddComponent<PolygonCollider2D>();
		LightingCollider2D collider = newGameObject.AddComponent<LightingCollider2D>();
        collider.shape.maskType = LightingCollider2D.MaskType.Collider;
        collider.shape.colliderType = LightingCollider2D.ColliderType.Collider;

		newGameObject.transform.position = GetCameraPoint();
    }

	#if UNITY_2017_4_OR_NEWER

	[MenuItem("GameObject/2D Light/Light Tilemap Collider", false, 4)]
    static void CreateLightTilemapCollider() {
		GameObject newGrid = new GameObject("2D Light Grid");
		newGrid.AddComponent<Grid>();

		GameObject newGameObject = new GameObject("2D Light Tilemap");
		newGameObject.transform.parent = newGrid.transform;

		newGameObject.AddComponent<Tilemap>();
		newGameObject.AddComponent<LightingTilemapCollider2D>();
    }

	#endif

	[MenuItem("GameObject/2D Light/Light Sprite Renderer", false, 4)]
    static void CreateLightSpriteRenderer() {
		GameObject newGameObject = new GameObject("2D Light Sprite Renderer");
		
		LightingSpriteRenderer2D spriteRenderer2D = newGameObject.AddComponent<LightingSpriteRenderer2D>();
        spriteRenderer2D.sprite = Resources.Load<Sprite>("Sprites/gfx_light");

		newGameObject.transform.position = GetCameraPoint();
    }

	[MenuItem("GameObject/2D Light/Light Texture Renderer", false, 4)]
    static void CreateLightTextureRenderer() {
		GameObject newGameObject = new GameObject("2D Light Texture Renderer");
		
		LightingTextureRenderer2D textureRenderer = newGameObject.AddComponent<LightingTextureRenderer2D>();
        textureRenderer.texture = Resources.Load<Texture>("Sprites/gfx_light");

		newGameObject.transform.position = GetCameraPoint();
    }

	[MenuItem("GameObject/2D Light/Day Light Collider", false, 4)]
    static void CreateDayLightCollider() {
		GameObject newGameObject = new GameObject("2D Light Day Collider");

		newGameObject.AddComponent<PolygonCollider2D>();

		DayLightingCollider2D c = newGameObject.AddComponent<DayLightingCollider2D>();
		c.shape.colliderType = DayLightingCollider2D.ColliderType.Collider;
		c.shape.maskType = DayLightingCollider2D.MaskType.None;

		newGameObject.transform.position = GetCameraPoint();
    }

	#if UNITY_2017_4_OR_NEWER

	[MenuItem("GameObject/2D Light/Day Light Tilemap Collider", false, 4)]
    static void CreateDayLightTilemapCollider() {
		GameObject newGrid = new GameObject("2D Light Grid");
		newGrid.AddComponent<Grid>();

		GameObject newGameObject = new GameObject("2D Day Light Tilemap");
		newGameObject.transform.parent = newGrid.transform;

		newGameObject.AddComponent<Tilemap>();
		newGameObject.AddComponent<DayLightingTilemapCollider2D>();
    }

	#endif

	
	[MenuItem("GameObject/2D Light/Light Room", false, 4)]
    static void CreateLightRoom() {
		GameObject newGameObject = new GameObject("2D Light Room");

		newGameObject.AddComponent<PolygonCollider2D>();
		newGameObject.AddComponent<LightingRoom2D>();

		newGameObject.transform.position = GetCameraPoint();
    }

	#if UNITY_2017_4_OR_NEWER

	[MenuItem("GameObject/2D Light/Light Tilemap Room", false, 4)]
    static void CreateLightTilemapRoom() {
		GameObject newGrid = new GameObject("2D Light Grid");
		newGrid.AddComponent<Grid>();

		GameObject newGameObject = new GameObject("2D Light Tilemap Room");
		newGameObject.transform.parent = newGrid.transform;

		newGameObject.AddComponent<Tilemap>();
		newGameObject.AddComponent<LightingTilemapRoom2D>();
    }

	#endif

	[MenuItem("GameObject/2D Light/Light Occlusion", false, 4)]
    static void CreateLightOcclusion() {
		GameObject newGameObject = new GameObject("2D Light Occlusion");

		newGameObject.AddComponent<PolygonCollider2D>();
		newGameObject.AddComponent<LightingOcclusion2D>();

		newGameObject.transform.position = GetCameraPoint();
    }

	#if UNITY_2017_4_OR_NEWER

	[MenuItem("GameObject/2D Light/Light Tilemap Occlusion", false, 4)]
    static void CreateLightTilemapOcclusion() {
		GameObject newGrid = new GameObject("2D Light Grid");
		newGrid.AddComponent<Grid>();

		GameObject newGameObject = new GameObject("2D Light Tilemap Occlusion");
		newGameObject.transform.parent = newGrid.transform;

		newGameObject.AddComponent<Tilemap>();
		newGameObject.AddComponent<LightingTilemapOcclusion2D>();
    }

	#endif

	[MenuItem("GameObject/2D Light/Light Settings", false, 4)]
    static void CreateLightSettings() {
		GameObject newGameObject = new GameObject("2D Light Settings");

		newGameObject.AddComponent<LightingSettings2D>();
    }

	[MenuItem("GameObject/2D Light/Light Manager", false, 4)]
    static void CreateLightManager(){
		LightingManager2D.Get();
    }
}
