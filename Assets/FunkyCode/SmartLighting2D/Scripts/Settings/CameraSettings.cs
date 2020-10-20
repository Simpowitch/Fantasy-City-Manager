using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[System.Serializable]
public struct CameraSettings {
	public int id;

	public enum RenderMode { Draw, Hidden, Disabled }
	public enum CameraType {MainCamera, Custom, SceneView};
	public enum RenderShader {MultiplyHDR, Multiply, Additive, Custom};
	
	public CameraType cameraType;
	public Camera customCamera;
	public RenderMode renderMode;
	public RenderShader renderShader;
	public Material customMaterial;

	public int bufferID;

	public CameraSettings(int id = 0) {
		this.id = id;

		renderMode = RenderMode.Draw;

		cameraType = CameraType.MainCamera;

		renderShader = RenderShader.Multiply;

		customMaterial = null;

		bufferID = 0;

		customCamera = null;
	}

	public Camera GetCamera() {
		Camera camera = null;
		switch(cameraType) {
			case CameraSettings.CameraType.MainCamera:
				camera = Camera.main;

				if (camera != null) {
					if (camera.orthographic == false) {
						return(null);
					}
				}

				return(Camera.main);

			case CameraSettings.CameraType.Custom:
				camera = customCamera;

				if (camera != null) {
					if (camera.orthographic == false) {
						return(null);
					}
				}

				return(customCamera);


            case CameraSettings.CameraType.SceneView:
			
				#if UNITY_EDITOR
					SceneView sceneView = SceneView.lastActiveSceneView;

					if (sceneView != null) {
						camera = sceneView.camera;

						#if UNITY_2019_1_OR_NEWER
						
							if (SceneView.lastActiveSceneView.sceneLighting == false) {
								camera = null;
							}

						#else
						
							if (SceneView.lastActiveSceneView.m_SceneLighting == false) {
								camera = null;
							}

						#endif
					}
	
					if (camera != null && camera.orthographic == false) {
						camera = null;
					}

					if (camera != null) {
						if (camera.orthographic == false) {
							return(null);
						}
					}

					return(camera);

				#else
					return(null);

				#endif
				
		}

		return(null);
	}

	public bool Equals(CameraSettings obj) {
        return this.bufferID == obj.bufferID && this.customCamera == obj.customCamera && this.cameraType == obj.cameraType;
    }

	public override int GetHashCode() {
        return this.GetHashCode();
    }
}