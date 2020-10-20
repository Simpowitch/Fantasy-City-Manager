using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LightingSettings;

[ExecuteInEditMode]
public class LightingBuffer2D {
	public string name = "unknown";

	public LightingSource2D lightSource;
	public LightTexture renderTexture;

	public bool updateNeeded = false;

	// Lighting Atlas Batching
	public LightingAtlasBatches lightingAtlasBatches = new LightingAtlasBatches();

	public static List<LightingBuffer2D> list = new List<LightingBuffer2D>();

	public LightingBuffer2D() {
		list.Add(this);
	}

	public static void Clear() {
		foreach(LightingBuffer2D buffer in new List<LightingBuffer2D>(list)) {
			if (buffer.lightSource) {
				buffer.lightSource.Buffer = null;
			}
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

	private bool free = true;
	public bool Free {
		get => free;
		set {
			free = value;

			Rendering.LightingBuffer.UpdateName(this);
		}
	}

	static public List<LightingBuffer2D> GetList() {
		return(list);
	}

	public void Initiate (int textureSize) {
		Rendering.LightingBuffer.InitializeRenderTexture(this, textureSize);
	}

	public void Render() {
		if (updateNeeded == false) {
			return;
		}
		
		updateNeeded = false;

		RenderTexture previous = RenderTexture.active;

		RenderTexture.active = renderTexture.renderTexture;

		if (lightSource.litMode == LightingSource2D.LitMode.Everything) {
			GL.Clear( false, true, Color.white);
		} else {
			GL.Clear( false, true, Color.black);
		}
	
		Rendering.LightingBuffer.Render(this);

		RenderTexture.active = previous;
	}
}