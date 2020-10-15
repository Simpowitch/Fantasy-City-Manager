using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class LightingMainBuffer2D {
	public string name = "Uknown";

	private LightingMaterial material = null;

	public bool updateNeeded = false;

	public LightTexture renderTexture;
	public CameraSettings cameraSettings;

	public static List<LightingMainBuffer2D> list = new List<LightingMainBuffer2D>();

	public LightingMainBuffer2D() {
		list.Add(this);
	}

	public static void Clear() {
		foreach(LightingMainBuffer2D buffer in new List<LightingMainBuffer2D>(list)) {
			buffer.DestroySelf();
		}

		list.Clear();
	}

	public void DestroySelf() {
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

	public bool IsActive() {
		return(list.IndexOf(this) > -1);
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

		CameraBuffers.Get();

		LightingMainBuffer2D buffer = new LightingMainBuffer2D();
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

	public void ClearMaterial() {
		material = null;
	}

	public Material GetMaterial() {
		if (material == null || material.Get() == null) {
			switch(cameraSettings.renderShader) {
				case CameraSettings.RenderShader.Multiply:
					if (Lighting2D.commonSettings.HDR) {
						material = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
					} else {
						material = LightingMaterial.Load("SmartLighting2D/Multiply");
					}
				break;

				case CameraSettings.RenderShader.Additive:
					
					material = LightingMaterial.Load(Max2D.shaderPath + "Particles/Additive");
					
				break;

				case CameraSettings.RenderShader.Custom:

					material = LightingMaterial.Load(cameraSettings.customMaterial);

				break;
			}
		}

		material.SetTexture(renderTexture.renderTexture);

		return(material.Get());
	}

	public void Update() {
		Rendering.LightingMainBuffer.Update(this);
	}

	public void Render() {
		if (cameraSettings.renderMode == CameraSettings.RenderMode.Disabled) {
			return;
		}

		if (updateNeeded) {
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture.renderTexture;
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