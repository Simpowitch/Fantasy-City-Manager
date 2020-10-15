using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarBuffer2D : LightingMonoBehaviour {

    public static List<FogOfWarBuffer2D> list = new List<FogOfWarBuffer2D>();

    public CameraSettings cameraSettings;
    public LightTexture renderTexture;

	public bool updateNeeded = false;

    LightingMaterial material = null;

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		if (renderTexture != null) {
			if (renderTexture.renderTexture != null) {
				if (Application.isPlaying) {
					UnityEngine.Object.Destroy (renderTexture.renderTexture);
				} else {
					UnityEngine.Object.DestroyImmediate (renderTexture.renderTexture);
				}
			}
		}
		
		list.Remove(this);
	}

    public Material GetMaterial() {
		if (material == null || material.Get() == null) {
            material = LightingMaterial.Load("SmartLighting2D/AlphaMask");
		}

		LightingManager2D manager = LightingManager2D.Get();
		CameraSettings[] cameraSettings = manager.cameraSettings;

		if (Lighting2D.fogOfWar.bufferID < cameraSettings.Length) {
			CameraSettings cameraSetting = cameraSettings[Lighting2D.fogOfWar.bufferID];

			LightingMainBuffer2D buffer = LightingMainBuffer2D.Get(cameraSetting);

			if (buffer != null) {
				Texture textureAlpha = buffer.renderTexture.renderTexture;

				Material mat = material.Get();

				if (renderTexture != null) {
					mat.mainTexture = renderTexture.renderTexture;  
				}

				mat.SetTexture("_Mask", textureAlpha);
			}
		}

		return(material.Get());
	}

    static public FogOfWarBuffer2D Get(CameraSettings cameraSettings) {
        foreach(FogOfWarBuffer2D FoWBuffer in list) {
            if (FoWBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera() && FoWBuffer.cameraSettings.bufferID == cameraSettings.bufferID) {
                return(FoWBuffer);
            }
        }

        GameObject gameObject = new GameObject ();
        gameObject.transform.parent = LightingManager2D.Get().transform;
        gameObject.name = "Fog of War Buffer";

        FogOfWarBuffer2D buffer = gameObject.AddComponent<FogOfWarBuffer2D> ();
        buffer.cameraSettings = cameraSettings;
        buffer.SetUpRenderTexture ();
        
        return(buffer);
    }

	public void SetUpRenderTexture() {
		Vector2Int screen = GetScreen();

		if (screen.x > 0 && screen.y > 0) {
            name = "Fog of War Buffer";

			renderTexture = new LightTexture (screen.x, screen.y, 32); // depth 24
			renderTexture.Create ();
		}
	}

	public Vector2Int GetScreen() {
		Camera camera = cameraSettings.GetCamera();

		if (camera == null) {
			return(Vector2Int.zero);
		}

		int screenWidth = (int)(camera.pixelRect.width * Lighting2D.fogOfWar.resolution);
		int screenHeight = (int)(camera.pixelRect.height * Lighting2D.fogOfWar.resolution);

		Vector2Int screen = new Vector2Int(screenWidth, screenHeight);

		return(screen);
	}

	public bool CameraSettingsCheck () {
		LightingManager2D manager = LightingManager2D.Get();
		int settingsID = cameraSettings.id;

		if (settingsID >= manager.cameraSettings.Length) {
			return(false);
		}

		if (manager.cameraSettings[settingsID].Equals(cameraSettings) == false) {
			return(false);
		}

		if (cameraSettings.GetCamera() == null) {
			return(false);
		}

		if (cameraSettings.bufferID != Lighting2D.fogOfWar.bufferID) {
			return(false);
		}

		cameraSettings.renderMode = manager.cameraSettings[settingsID].renderMode;

		return(true);
	}

    public void Update() {
		Rendering.FogOfWarBuffer.Check.RenderTexture(this);

		Rendering.FogOfWarBuffer.LateUpdate(this);

		Rendering.FogOfWarBuffer.DrawOn(this);
    }

	public void Render() {
		if (updateNeeded) {
	
			if (renderTexture != null) {
				RenderTexture previous = RenderTexture.active;

				RenderTexture.active = renderTexture.renderTexture;
				GL.Clear( false, true, new Color(0, 0, 0, 0));

				Rendering.FogOfWarBuffer.Render(this);

				RenderTexture.active = previous;
			}
			
		}
	}

	// Apply Render to Specified Camera (Post Render Mode)
	private void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}

		if (Lighting2D.renderingMode != RenderingMode.OnPostRender) {
			return;
		}

		// if (cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
		// 	return;
		// }

		FogOfWarRender.PostRender(this);
	}
}