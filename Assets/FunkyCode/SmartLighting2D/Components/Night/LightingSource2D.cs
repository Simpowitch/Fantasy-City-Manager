using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;

[ExecuteInEditMode]
public class LightingSource2D : LightingMonoBehaviour{
	public enum LightSprite {Default, Custom};
	public enum WhenInsideCollider {DrawAbove, DrawInside}; // Draw Bellow / Do Not Draw
	public enum LitMode {Everything, MaskOnly}

	// Settings
	public LightingLayer nightLayer = LightingLayer.Layer1;
	public Color color = new Color(.5f,.5f, .5f, 1);

	public float size = 5f;
	public float coreSize = 0.5f;
	public float angle = 360;
	public float outerAngle = 15;
	public LitMode litMode = LitMode.Everything;
	
	public bool applyRotation = false;

	public int lightPresetId = 0;

	// public float lightCoreSize = 0.5f;

	public LightingSourceTextureSize textureSize = LightingSourceTextureSize.px2048;

	public MeshMode meshMode = new MeshMode();
	public BumpMap bumpMap = new BumpMap();

	public WhenInsideCollider whenInsideCollider = WhenInsideCollider.DrawInside;

	public LightSprite lightSprite = LightSprite.Default;
	private static Sprite defaultSprite = null;
	public Sprite sprite;
	public bool spriteFlipX = false;
	public bool spriteFlipY = false;

	
	public LightingBuffer2D Buffer {
		get => buffer;
		set => buffer = value;
	}
	private LightingBuffer2D buffer = null;

	public LightingSourceTransform transform2D = new LightingSourceTransform();
	private static List<LightingSource2D> list = new List<LightingSource2D>();
	
	private bool inScreen = false;

	public LightEventHandling eventHandling = new LightEventHandling();

	[System.Serializable]
	public class LightEventHandling {
		public bool enable = false;

		public bool useColliders = true;
		public bool useTilemapColliders = false;

		public EventHandling.Object eventHandlingObject = new EventHandling.Object();
	}

	[System.Serializable]
	public class BumpMap {
		public float intensity = 1;
		public float depth = 1;
	}

	public LayerSetting[] GetLayerSettings() {
		LightPresetList presetList = Lighting2D.Profile.lightPresets;
		
		if (lightPresetId >= presetList.list.Length) {
			return(null);
		}

		LightPreset lightPreset = presetList.Get()[lightPresetId];

		return(lightPreset.layerSetting.Get());
	}

	static public Sprite GetDefaultSprite() {
		if (defaultSprite == null || defaultSprite.texture == null) {
			defaultSprite = Resources.Load <Sprite> ("Sprites/gfx_light");
		}
		return(defaultSprite);
	}

	public Sprite GetSprite() {
		if (sprite == null || sprite.texture == null) {
			sprite = GetDefaultSprite();
		}
		return(sprite);
	}

	public void ForceUpdate() {
		transform2D.ForceUpdate();
	}

	static public void ForceUpdateAll() {
		foreach(LightingSource2D light in LightingSource2D.GetList()) {
			light.ForceUpdate();
		}
	}

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		transform2D.ForceUpdate();

		ForceUpdate();
	}

	public void OnDisable() {
		list.Remove(this);

		Free();
	}

	public void Free() {
		LightBuffers.FreeBuffer(buffer);

		inScreen = false;
	}

	static public List<LightingSource2D> GetList() {
		return(list);
	}

	public bool InAnyCamera() {
		LightingManager2D manager = LightingManager2D.Get();
		CameraSettings[] cameraSettings = manager.cameraSettings;

		for(int i = 0; i < cameraSettings.Length; i++) {
			Camera camera = manager.GetCamera(i);

			if (camera == null) {
				continue;
			}

			// if application is running
			//if (cameraSettings[i].cameraType == CameraSettings.CameraType.SceneView) {
			//	continue;
			////}

			float dist = Vector2.Distance(transform.position, camera.transform.position);
			float cameraSize = camera.orthographicSize;
			float cameraSize2 = (cameraSize * 2f);
			float diameter = Mathf.Sqrt(cameraSize2 * cameraSize2) + this.size;

			if (dist < diameter) {
				return(true);
			}
		}

		return(false);
	}

	public LightingBuffer2D GetBuffer() {
		if (buffer == null) { //?
			int textureSizeInt = LightingRender2D.GetTextureSize(textureSize);
			buffer = LightBuffers.PullBuffer (textureSizeInt, this);
		}
		
		return(buffer);
	}

	public void UpdateLoop() {
		transform2D.Update(this);

		UpdateBuffer();

		DrawMeshMode();
	}

	void BufferUpdate() {
		transform2D.UpdateNeeded = false;

		if (Lighting2D.disable == true) {
			return;
		}

		if (buffer == null) {
			return;
		}
		
		buffer.updateNeeded = true;
	}

	void UpdateBuffer() {

		if (InAnyCamera()) {
			if (GetBuffer() == null) {
				return;
			}

			if (transform2D.UpdateNeeded == true || inScreen == false) {
				BufferUpdate();

				inScreen = true;
			}
			
		} else {
			if (buffer != null) {
				LightBuffers.FreeBuffer(buffer);
			}
			
			inScreen = false;
		}
		
		if (eventHandling.enable) {
			eventHandling.eventHandlingObject.Update(this, eventHandling.useColliders, eventHandling.useTilemapColliders);
		}
	}

	public void DrawMeshMode() {
		if (meshMode.enable == false) {
			return;
		}

		if (buffer == null) {
			return;
		}

		if (isActiveAndEnabled == false) {
			return;
		}

		if (InAnyCamera() == false) {
			return;
		}
		
		LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);
		
		if (lightingMesh != null) {
			lightingMesh.UpdateLightSource(this, meshMode);
		}	
	}

	void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.sceneView.drawGizmos == false) {
			return;
		}
		
		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		Vector3 center = transform.position;
		int step = 10;

		int start = -(int)(angle / 2);
		int end = (int)(angle / 2);

		for(int i = start; i < end; i += step) {
			float rot = i + 90 + transform2D.rotation;

			Vector3 pointA = center;
			float rotA = rot * Mathf.Deg2Rad;
			pointA.x += Mathf.Cos(rotA) * size;
			pointA.y += Mathf.Sin(rotA) * size;


			Vector3 pointB = center;
			float rotB = (rot + step) * Mathf.Deg2Rad;
			pointB.x += Mathf.Cos(rotB) * size;
			pointB.y += Mathf.Sin(rotB) * size;

			Gizmos.DrawLine(pointA, pointB);

			if (angle < 360 && angle > 0) {
				if (i == start) {
					Gizmos.DrawLine(pointA, center);
				}

				if (i + step > end) {
					Gizmos.DrawLine(pointB, center);
				}
			}
		}
    }

	private void OnDrawGizmos() {
		if (Lighting2D.ProjectSettings.sceneView.drawGizmos == false) {
			return;
		}
		
		Gizmos.DrawIcon(transform.position, "light", true);
	}
}