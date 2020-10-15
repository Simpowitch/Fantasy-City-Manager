using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode]
public class LightingSpriteRenderer2D : MonoBehaviour {
	public enum Type {Particle, Mask};
	public enum SpriteMode {Custom, SpriteRenderer};

	public LightingLayer nightLayer = LightingLayer.Layer1;
	public Type type = Type.Particle;
	public SpriteMode spriteMode = SpriteMode.Custom;
	public Sprite sprite = null;

    public Color color = new Color(0.5f, 0.5f, 0.5f, 1f);
	
    public bool flipX = false;
    public bool flipY = false;

	public TransformOffset transformOffset = new TransformOffset();

	public MeshMode meshMode = new MeshMode();

	public GlowMode glowMode = new GlowMode();

	public VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public static List<LightingSpriteRenderer2D> list = new List<LightingSpriteRenderer2D>();

	SpriteRenderer spriteRendererComponent;

	static public List<LightingSpriteRenderer2D> GetList() {
		return(list);
	}

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		color.a = 1f; // ?
	}

	public void OnDisable() {
		list.Remove(this);
	}

	public bool InCamera(Camera camera) {
		float cameraSize = camera.orthographicSize * 1.25f;

		float verticalSize = cameraSize;
        float horizontalSize = cameraSize * ((float)camera.pixelRect.width / camera.pixelRect.height);

		return(Vector2.Distance(transform.position, camera.transform.position) < Mathf.Sqrt((verticalSize) * (horizontalSize)) + GetSize() * 2 );
	}

	public Sprite GetSprite() {
		if (GetSpriteOrigin() == null) {
			return(null);
		}

		if (glowMode.enable) {
			Sprite blurredSprite = GlowManager.RequestSprite(GetSpriteOrigin(), glowMode.glowSize, glowMode.glowIterations);
			if (blurredSprite == null) {
				return(GetSpriteOrigin());
			} else {
				return(blurredSprite);
			}
		} else {
			return(GetSpriteOrigin());
		}
	}

	public Sprite GetSpriteOrigin() {
		if (spriteMode == SpriteMode.Custom) {
			return(sprite);
		} else {
			if (GetSpriteRenderer() == null) {
				return(null);
			}
			sprite = spriteRendererComponent.sprite;

			return(sprite);
		}
	}

	public SpriteRenderer GetSpriteRenderer() {
		if (spriteRendererComponent == null) {
			spriteRendererComponent = GetComponent<SpriteRenderer>();
		}
		return(spriteRendererComponent);
	}

	public void LateUpdate() {
		if (spriteMode == SpriteMode.SpriteRenderer) {
			SpriteRenderer sr = GetSpriteRenderer();
			if (sr != null) {
				spriteRenderer.flipX = sr.flipX;
				spriteRenderer.flipY = sr.flipY;		
			}
		} else {
			spriteRenderer.flipX = flipX;
			spriteRenderer.flipY = flipY;	
		}
		

		spriteRenderer.sprite = GetSprite();
		spriteRenderer.color = color;

		if (meshMode.enable) {
			DrawMesh();
		}
	}

	public void DrawMesh() {
		if (meshMode.enable) {
			LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);

			if (lightingMesh != null) {
				lightingMesh.UpdateLightSprite(this, meshMode);
			}
		}
	}

	float GetSize() {
		Vector2 size = transformOffset.offsetScale;

		Sprite sprite = GetSprite();

		Rect spriteRect = sprite.textureRect;

		float spriteSheetUV_X = (float)(sprite.texture.width) / spriteRect.width;
		float spriteSheetUV_Y = (float)(sprite.texture.height) / spriteRect.height;
		
		Vector2 scale = new Vector2(spriteSheetUV_X * spriteRect.width / sprite.pixelsPerUnit, spriteSheetUV_Y * spriteRect.height / sprite.pixelsPerUnit);

		scale.x = (float)sprite.texture.width / spriteRect.width;
		scale.y = (float)sprite.texture.height / spriteRect.height;

		size.x /= scale.x;
		size.y /= scale.y;

		size.x *= (float)sprite.texture.width / (sprite.pixelsPerUnit * 2);
		size.y *= (float)sprite.texture.height / (sprite.pixelsPerUnit * 2);
		
		Vector2 scale2 = transform.lossyScale;
		scale2.x *= size.x;
		scale2.y *= size.y;
		
		return(Mathf.Sqrt(scale2.x * scale2.x + scale2.y * scale2.y));
	}

	// Serializable Units
	[System.Serializable]
	public class TransformOffset {
		public bool applyTransformRotation = true;

		public Vector2 offsetScale = new Vector2(1, 1);
		public float offsetRotation = 0;
		public Vector2 offsetPosition = new Vector2(0, 0);	
	}
}
