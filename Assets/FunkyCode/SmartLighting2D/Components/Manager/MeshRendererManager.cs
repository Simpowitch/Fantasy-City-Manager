using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshRendererManager : MonoBehaviour {
    public static MeshRendererManager instance;

    public static MeshRendererManager Get() {
        if (instance != null) {
			return(instance);
		}

		foreach(MeshRendererManager meshModeObject in Object.FindObjectsOfType(typeof(MeshRendererManager))) {
			instance = meshModeObject;
			return(instance);
		}

        if (instance == null) {
            GameObject meshRendererMode = new GameObject("Mesh Renderer Manager");
            instance = meshRendererMode.AddComponent<MeshRendererManager>();
          
            LightingManager2D manager = LightingManager2D.Get();
            meshRendererMode.transform.parent = manager.transform;
        }

        return(instance);
    }

	public void Awake() {
		foreach(LightingMeshRenderer buffer in Object.FindObjectsOfType(typeof(LightingMeshRenderer))) {
			buffer.DestroySelf();
		}
	}

   // void LateUpdate() {
     //   UpdatePosition();
  //  }

    // Management
	static public LightingMeshRenderer AddBuffer(Object source) {
       // LightingManager2D manager = LightingManager2D.Get();
       // if (manager.fixedLightBufferSize) {
       //     textureSize = LightingManager2D.GetTextureSize(manager.fixedLightTextureSize);
       // }

		MeshRendererManager manager = Get();

		if (manager == null) {
			Debug.LogError("Lighting Manager Instance is Out-Dated.");
			Debug.LogError("Try Re-Initializing 'Lighting Manager 2D' Component");
			return(null);
		}

		GameObject buffer = new GameObject ();
		buffer.name = "Mesh Renderer (Id :" + (LightingMeshRenderer.GetCount() + 1) + ")";
		buffer.transform.parent = manager.transform;

		LightingMeshRenderer lightingBuffer2D = buffer.AddComponent<LightingMeshRenderer> ();
		
		lightingBuffer2D.Initialize ();

		lightingBuffer2D.owner = source;
		lightingBuffer2D.free = false;

		return(lightingBuffer2D);
	}

	public static LightingMeshRenderer Pull(Object source) {
		foreach (LightingMeshRenderer id in LightingMeshRenderer.GetList()) {
			if (id.owner == source) {
				id.gameObject.SetActive (true);
				return(id);
			}
		}

		foreach (LightingMeshRenderer id in LightingMeshRenderer.GetList()) {
			if (id.free == true) {
				id.free = false;
				id.owner = source;
				id.gameObject.SetActive (true);
				return(id);
			}
		}
			
		return(AddBuffer(source));		
	}
}