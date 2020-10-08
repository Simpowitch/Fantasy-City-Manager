using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarBuffer2D : LightingMonoBehaviour {

    public static List<FogOfWarBuffer2D> list = new List<FogOfWarBuffer2D>();

    public CameraSettings cameraSettings;
    public RenderTexture renderTexture;

	public bool updateNeeded = false;

    LightingMaterial material = null;

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

    public Material GetMaterial() {
		if (material == null || material.Get() == null) {
            material = LightingMaterial.Load("SmartLighting2D/AlphaMask");
		}

		LightingManager2D manager = LightingManager2D.Get();

		if (Lighting2D.fogOfWar.bufferID < manager.cameraSettings.Length) {
			CameraSettings cameraSettings = manager.cameraSettings[Lighting2D.fogOfWar.bufferID];

			LightingMainBuffer2D buffer = LightingMainBuffer2D.Get(cameraSettings);

			if (buffer != null) {
				Texture textureAlpha = buffer.renderTexture;

				material.Get().mainTexture = renderTexture;       
				material.Get().SetTexture("_Mask", textureAlpha);
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
        buffer.Initialize();
        
        return(buffer);
    }

    public void Initialize() {
		SetUpRenderTexture ();
	}

	void SetUpRenderTexture() {
		Camera camera = cameraSettings.GetCamera();

		if (camera == null) {
			return;
		}

		int screenWidth = (int)(camera.pixelRect.width * Lighting2D.fogOfWar.resolution);
		int screenHeight = (int)(camera.pixelRect.height * Lighting2D.fogOfWar.resolution);

		if (screenWidth > 0 && screenHeight > 0) {
            name = "Fog of War Buffer";

			renderTexture = new RenderTexture (screenWidth, screenHeight, 32);
            renderTexture.depth = 24;
			renderTexture.Create ();
		}
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
		Rendering.FogOfWarBuffer.LateUpdate(this);

		Rendering.FogOfWarBuffer.DrawOn(this);
    }

	public void Render() {

		if (updateNeeded) {
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture;
			GL.Clear( false, true, new Color(0, 0, 0, 0));

			Rendering.FogOfWarBuffer.Render(this);

			RenderTexture.active = previous;
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