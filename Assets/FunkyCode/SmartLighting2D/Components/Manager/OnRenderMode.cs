using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[ExecuteInEditMode]
public class OnRenderMode : LightingMonoBehaviour {
    public LightingMainBuffer2D mainBuffer;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public static List<OnRenderMode> list = new List<OnRenderMode>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);   
    }

    public static OnRenderMode Get(LightingMainBuffer2D buffer) {
		foreach(OnRenderMode meshModeObject in list) {
			if (meshModeObject.mainBuffer == buffer) {
                return(meshModeObject);
            }
		}

        GameObject meshRendererMode = new GameObject("On Render");
        OnRenderMode onRenderMode = meshRendererMode.AddComponent<OnRenderMode>();

        onRenderMode.mainBuffer = buffer;
        onRenderMode.Initialize(buffer);
        onRenderMode.UpdateLayer();

        onRenderMode.name = "On Render: " + buffer.name;

        return(onRenderMode);
    }

    public void Initialize(LightingMainBuffer2D mainBuffer) {         
        transform.parent = CameraBuffers.Get().transform;
        
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mainBuffer.GetMaterial();

        BufferPreset bufferPreset = mainBuffer.GetBufferPreset();
           
        bufferPreset.sortingLayer.ApplyToMeshRenderer(meshRenderer);

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

    void UpdateLayer() {
        if (mainBuffer == null || mainBuffer.IsActive() == false) {
            DestroySelf();
            return;
        }

        #if UNITY_EDITOR
            LightingManager2D manager = LightingManager2D.Get();

            if (manager != null && mainBuffer.cameraSettings.cameraType != CameraSettings.CameraType.SceneView) {
                gameObject.layer = Lighting2D.ProjectSettings.sceneView.layer;
            } else {
                gameObject.layer = 0;
            }
			
		#endif
    }

    void LateUpdate() {

        UpdateLayer();

        if (mainBuffer == null || mainBuffer.IsActive() == false) {
            DestroySelf();
            return;
        }

        if (mainBuffer.cameraSettings.GetCamera() == null) {
            DestroySelf();

            return;
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
		
        if (mainBuffer.cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
			meshRenderer.enabled = false;
		}
      
		if (Lighting2D.renderingMode == RenderingMode.OnRender) {
            UpdatePosition();
        }
    }

    public void UpdatePosition() {
        if (mainBuffer == null) {
            return;
        }
        
        Camera camera = mainBuffer.cameraSettings.GetCamera();
        
        if (camera == null) {
            return;
        }

        transform.position = LightingPosition.GetCamera(camera);
        transform.rotation = camera.transform.rotation;
        transform.localScale = LightingRender2D.GetSize(camera);
    }
}