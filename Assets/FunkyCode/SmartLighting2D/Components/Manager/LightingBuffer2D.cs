using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LightingSettings;

[ExecuteInEditMode]
public class LightingBuffer2D : LightingMonoBehaviour {
	public LightingSource2D lightSource;
	public RenderTexture renderTexture;

	public bool updateNeeded = false;

	// Lighting Atlas Batching
	public LightingAtlasBatches lightingAtlasBatches = new LightingAtlasBatches();

	public static List<LightingBuffer2D> list = new List<LightingBuffer2D>();

	private bool free = true;
	public bool Free {
		get => free;
		set {
			free = value;

			Rendering.LightingBuffer.UpdateName(this);
		}
	}

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingBuffer2D> GetList() {
		return(list);
	}

	public void Initiate (int textureSize) {
		Rendering.LightingBuffer.InitializeRenderTexture(this, textureSize);
	}

	public void Render() {
		Rendering.LightingBuffer.Update(this);

		Rendering.LightingBuffer.LateUpdate(this);

		if (updateNeeded) {
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture;
			GL.Clear( false, true, Color.white);

			Rendering.LightingBuffer.Render(this);

			RenderTexture.active = previous;

			updateNeeded = false;
		}
	}
}