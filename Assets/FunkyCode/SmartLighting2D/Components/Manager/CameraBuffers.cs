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

		return(instance);
	}
}
