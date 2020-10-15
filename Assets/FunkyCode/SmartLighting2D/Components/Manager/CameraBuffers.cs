using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBuffers : MonoBehaviour {
    static private CameraBuffers instance;

	public void Awake() {
		foreach(OnRenderMode onRenderMode in Object.FindObjectsOfType(typeof(OnRenderMode))) {
			onRenderMode.DestroySelf();
		}
	}

    static public CameraBuffers Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(CameraBuffers root in Object.FindObjectsOfType(typeof(CameraBuffers))) {
			instance = root;
			return(instance);
		}

		LightingManager2D manager = LightingManager2D.Get();

		GameObject gameObject = new GameObject ();
		gameObject.transform.parent = manager.transform;
		gameObject.name = "Camera Buffers";

		instance = gameObject.AddComponent<CameraBuffers> ();
		instance.GetCamera();

		return(instance);
	}

	private Camera lightingCamera = null;

	private void OnEnable() {
		SetUpCamera();
	}

	public Camera GetCamera() {
		if (lightingCamera == null) {
			lightingCamera = gameObject.GetComponent<Camera>();

			if (lightingCamera == null) {
				lightingCamera = gameObject.AddComponent<Camera>();
				SetUpCamera();
			}
		}

		return(lightingCamera);
	}

	#if UNITY_EDITOR
		private void Update() {
			LightingManager2D manager = LightingManager2D.Get();

			if (manager != null) {
				gameObject.layer = Lighting2D.ProjectSettings.sceneView.layer;
			}
		}
	#endif

	void SetUpCamera() {
		if (lightingCamera == null) {
			return;
		}
		
		lightingCamera.clearFlags = CameraClearFlags.Nothing;
		lightingCamera.backgroundColor = Color.white;
		lightingCamera.cameraType = CameraType.Game;
		lightingCamera.orthographic = true;
		lightingCamera.farClipPlane = 0;
		lightingCamera.nearClipPlane = 0f;
		lightingCamera.allowHDR = false;
		lightingCamera.allowMSAA = false;
		lightingCamera.enabled = false;
		lightingCamera.depth = -100;
		lightingCamera.orthographicSize = 0.1f;
	}

    private void OnPreCull() {
        if (Lighting2D.Profile.qualitySettings.updateMethod == LightingSettings.QualitySettings.UpdateMethod.OnPreCull) {
            LightingManager2D manager = LightingManager2D.Get();
            manager.UpdateLoop();
        }
    }
}
