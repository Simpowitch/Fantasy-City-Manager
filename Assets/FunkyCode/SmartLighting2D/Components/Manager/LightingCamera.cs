using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingCamera : MonoBehaviour
{
    static private LightingCamera instance;
    public Camera lightingCamera;

    static public LightingCamera Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightingCamera root in Object.FindObjectsOfType(typeof(LightingCamera))) {
			instance = root;
			return(instance);
		}

		LightingManager2D manager = LightingManager2D.Get();

		GameObject gameObject = new GameObject ();
		gameObject.transform.parent = manager.transform;
		gameObject.name = "Camera";

		instance = gameObject.AddComponent<LightingCamera> ();
        instance.SetUpCamera();

		return(instance);
	}

	private void Update() {
		#if UNITY_EDITOR
            LightingManager2D manager = LightingManager2D.Get();

            if (manager != null) {
                gameObject.layer = manager.gameObject.layer;
            }
		#endif
	}

    private void OnPreCull() {
        
        if (Lighting2D.Profile.qualitySettings.updateMethod == LightingSettings.QualitySettings.UpdateMethod.OnPreCull) {
            LightingManager2D manager = LightingManager2D.Get();
            manager.UpdateLoop();
        }

    }

    void SetUpCamera() {
		lightingCamera = gameObject.AddComponent<Camera>();
		lightingCamera.clearFlags = CameraClearFlags.Color;
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
}
