using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarOnRenderMode : LightingMonoBehaviour {
    public Camera regularCamera;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public static List<FogOfWarOnRenderMode> list = new List<FogOfWarOnRenderMode>();

	public void OnEnable() {
		list.Add(this);

        Lighting2D.fogOfWar.sortingLayer.ApplyToMeshRenderer(GetMeshRenderer());
	}

	public void OnDisable() {
		list.Remove(this);
	}

    public MeshRenderer GetMeshRenderer() {
        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        return(meshRenderer);
    }

    public static FogOfWarOnRenderMode Get(Camera camera, FogOfWarBuffer2D mainBuffer) {
		foreach(FogOfWarOnRenderMode meshModeObject in list) {
			if (meshModeObject.regularCamera == camera) {
                return(meshModeObject);
            }
		}

        GameObject meshRendererMode = new GameObject("On Render");
        FogOfWarOnRenderMode onRenderMode = meshRendererMode.AddComponent<FogOfWarOnRenderMode>();

        onRenderMode.regularCamera = camera;
        onRenderMode.Initialize(camera, mainBuffer);

        return(onRenderMode);
    }

    public void Initialize(Camera camera, FogOfWarBuffer2D mainBuffer) {         
        transform.parent = mainBuffer.transform;
        
        GetMeshRenderer();
        meshRenderer.sharedMaterial = new Material(Shader.Find("SmartLighting2D/AlphaMask"));
           
        // Disable Mesh Renderer Settings
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.allowOcclusionWhenDynamic = false;

        UpdatePosition();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = LightingRender2D.GetMesh();
    }

    public void UpdatePosition() {
        Camera camera = regularCamera;
        if (camera == null) {
            return;
        }
        
        Vector3 position = camera.transform.position;
        position.z += camera.nearClipPlane + 0.2f;

        transform.position = position;
        transform.rotation = camera.transform.rotation;
        transform.localScale = LightingRender2D.GetSize(regularCamera);
    }

    
    public void Update() {
        if (regularCamera == null) {
            DestroySelf();
        }

        if (Lighting2D.renderingMode != RenderingMode.OnRender) {
            DestroySelf();

            return;
		}

         if (Lighting2D.disable) {
            if (meshRenderer != null) {
				meshRenderer.enabled = false;
			}
        }
        
        if (Lighting2D.fogOfWar.enabled == false) {
			meshRenderer.enabled = false;
		}
			
		if (Lighting2D.renderingMode != RenderingMode.OnRender) {
			meshRenderer.enabled = false;
		}
    }

    void LateUpdate() {
		if (Lighting2D.renderingMode == RenderingMode.OnRender) {
            UpdatePosition();
        }
    }
}
