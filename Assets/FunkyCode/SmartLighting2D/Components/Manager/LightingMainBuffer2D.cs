using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class LightingMainBuffer2D : LightingMonoBehaviour {
	private LightingMaterial material = null;

	public bool updateNeeded = false;

	public RenderTexture renderTexture;
	public CameraSettings cameraSettings;

	public static List<LightingMainBuffer2D> list = new List<LightingMainBuffer2D>();

	private void OnEnable() {
		list.Add(this);
	}

	private void OnDisable() {
		list.Remove(this);
	}

	static public LightingMainBuffer2D Get(CameraSettings cameraSettings) {
		if (cameraSettings.GetCamera() == null) {
			return(null);
		}

		foreach(LightingMainBuffer2D mainBuffer in list) {
			if (mainBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera() && mainBuffer.cameraSettings.bufferID == cameraSettings.bufferID) {
				return(mainBuffer);
			}
		}

		if (Lighting2D.bufferPresets.Length <= cameraSettings.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		GameObject gameObject = new GameObject ("Camera Buffer");
		gameObject.transform.parent = CameraBuffers.Get().transform;

		LightingMainBuffer2D buffer = gameObject.AddComponent<LightingMainBuffer2D> ();
		buffer.cameraSettings = cameraSettings;

		Rendering.LightingMainBuffer.InitializeRenderTexture(buffer);

		return(buffer);
	}

	public BufferPreset GetBufferPreset() {
		if (Lighting2D.bufferPresets.Length <= cameraSettings.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		return(Lighting2D.bufferPresets[cameraSettings.bufferID]);
	}

	public Material GetMaterial() {
		if (material == null || material.Get() == null) {
			if (Lighting2D.commonSettings.HDR) {
				material = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
			} else {
				material = LightingMaterial.Load(Max2D.shaderPath + "Particles/Multiply");
			}
		}

		material.SetTexture(renderTexture);

		return(material.Get());
	}

	public void Update() {
		Rendering.LightingMainBuffer.LateUpdate(this);
	}

	public void Render() {
		if (updateNeeded) {
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture;
			GL.Clear( false, true, Color.white);

			Rendering.LightingMainBuffer.Render(this);

			RenderTexture.active = previous;
		}


		Rendering.LightingMainBuffer.DrawOn(this);
	}

	// Apply Render to Specified Camera (Post Render Mode)
	private void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}
		
		Rendering.LightingMainBuffer.DrawPost(this);
	}
}