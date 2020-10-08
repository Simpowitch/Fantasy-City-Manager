using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightingMeshRenderer : LightingMonoBehaviour {
 	public static List<LightingMeshRenderer> list = new List<LightingMeshRenderer>();
	
	public bool free = true;
	public Object owner = null;

	public MeshRenderer meshRenderer = null;
	public MeshFilter meshFilter = null;

	private LightingMaterial material = null;

	public Material GetMaterial() {
		if (material == null) {
			material = LightingMaterial.Load(Max2D.shaderPath + "Particles/Additive");
		}
		return(material.Get());
	}

	static public int GetCount() {
		return(list.Count);
	}

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingMeshRenderer> GetList() {
		return(list);
	}

	public void Initialize() {
		meshFilter = gameObject.AddComponent<MeshFilter>();
     
	 	// Mesh System?
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.allowOcclusionWhenDynamic = false;	
	}

	public void Free() {
		owner = null;
		free = true;

		meshRenderer.enabled = false;
		
		if (meshRenderer.sharedMaterial != null) {
			meshRenderer.sharedMaterial.mainTexture = null;
		}
	}

	public void LateUpdate() {
		if (owner == null) {
			Free();
			return;
		}

		LightingManager2D manager = LightingManager2D.Get();


		#if UNITY_EDITOR

			gameObject.layer = manager.gameObject.layer;
			
		#endif

		string type = owner.GetType().ToString();

		switch(type) {
			case "LightingSource2D":
				LightingSource2D source = (LightingSource2D)owner;
				if (source) {
					if (source.additiveMode.enable == false) {
						Free();
						return;
					}
					if (source.isActiveAndEnabled == false) {
						Free();
					} else {
						meshRenderer.enabled = true;
					}
				}
			break;

			case "LightingSpriteRenderer2D":
				LightingSpriteRenderer2D source2 = (LightingSpriteRenderer2D)owner;
				if (source2) {
					if (source2.additiveMode.enable == false) {
						Free();
						return;
					}
					if (source2.isActiveAndEnabled == false) {
						Free();
					} else {
						meshRenderer.enabled = true;
					}
				}

			break;
		}
	}

	public void UpdateLightSource(LightingSource2D id) {
		transform.position = id.transform.position;
		transform.localScale = new Vector3(id.size, id.size, 1);
		transform.rotation = Quaternion.Euler(0, 0, 0);
		// transform.rotation = id.transform.rotation; // only if rotation enabled
	
		if (id.Buffer != null && meshRenderer != null) {
			Color lightColor = id.color;
			lightColor.a = id.additiveMode.alpha;

			GetMaterial().SetColor ("_TintColor", lightColor);
			GetMaterial().mainTexture = id.Buffer.renderTexture;

			id.additiveMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

			meshRenderer.sharedMaterial = GetMaterial();

			meshRenderer.enabled = true;

			meshFilter.mesh = GetRenderMeshLight();
		}
	}

	public void UpdateLightSprite(LightingSpriteRenderer2D id) {
		if (id.GetSprite() == null) {
			Free();
			return;
		}

		float rotation = id.transformOffset.offsetRotation;
		if (id.transformOffset.applyTransformRotation) {
			rotation += id.transform.rotation.eulerAngles.z;
		}

		////////////////////// Scale
		Vector2 scale = Vector2.zero;

		Sprite sprite = id.GetSprite();

		Rect spriteRect = sprite.textureRect;

		scale.x = (float)sprite.texture.width / spriteRect.width;
		scale.y = (float)sprite.texture.height / spriteRect.height;

		Vector2 size = id.transformOffset.offsetScale;

		if (id.glowMode.enable) {
			size.x *= 2;
			size.y *= 2;
		}

		size.x /= scale.x;
		size.y /= scale.y;

		size.x *= (float)sprite.texture.width / (sprite.pixelsPerUnit * 2);
		size.y *= (float)sprite.texture.height / (sprite.pixelsPerUnit * 2);
		
		if (id.spriteRenderer.flipX) {
			size.x = -size.x;
		}

		if (id.spriteRenderer.flipY) {
			size.y = -size.y;
		}

		////////////////////// PIVOT
		Rect rect = spriteRect;
		Vector2 pivot = sprite.pivot;

		pivot.x /= spriteRect.width;
		pivot.y /= spriteRect.height;
		pivot.x -= 0.5f;
		pivot.y -= 0.5f;
		
		if (id.glowMode.enable) {
			pivot.x *= size.x;
			pivot.y *= size.y;
		} else {
			pivot.x *= size.x * 2;
			pivot.y *= size.y * 2;
		}
	
		float pivotDist = Mathf.Sqrt(pivot.x * pivot.x + pivot.y * pivot.y);
		float pivotAngle = Mathf.Atan2(pivot.y, pivot.x);

		float rot = rotation * Mathf.Deg2Rad + Mathf.PI;

		Vector2 position = Vector2.zero;

		// Pivot Pushes Position
		
		position.x += Mathf.Cos(pivotAngle + rot) * pivotDist * id.transform.lossyScale.x;
		position.y += Mathf.Sin(pivotAngle + rot) * pivotDist * id.transform.lossyScale.y;
		position.x += id.transform.position.x;
		position.y += id.transform.position.y;
		position.x += id.transformOffset.offsetPosition.x;
		position.y += id.transformOffset.offsetPosition.y;

		Vector3 pos = position;
		pos.z = id.transform.position.z - 0.1f;
		transform.position = pos;

		Vector3 scale2 = id.transform.lossyScale;

		scale2.x *= size.x;
		scale2.y *= size.y;

		if (id.glowMode.enable) {
			scale2.x /= 2;
			scale2.y /= 2;
		}

		scale2.z = 1;

       	// transform.rotation = id.transform.rotation; // only if rotation enabled
		transform.localScale = scale2;
		transform.rotation = Quaternion.Euler(0, 0, rotation);

		Rect uvRect = new Rect();
		uvRect.x = rect.x / sprite.texture.width;
		uvRect.y = rect.y / sprite.texture.height;
		uvRect.width = rect.width / sprite.texture.width + uvRect.x;
		uvRect.height = rect.height / sprite.texture.height + uvRect.y;
	
		if (meshRenderer != null) {
			Color lightColor = id.color;
			lightColor.a = id.additiveMode.alpha;

			GetMaterial().mainTexture = id.GetSprite().texture;

			GetMaterial().SetColor ("_TintColor", lightColor);

			id.additiveMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

			Material material = GetMaterial();
			//material.mainTexture = id.sprite.texture;

			meshRenderer.sharedMaterial = material;

			meshRenderer.enabled = true;
		
			Mesh mesh = GetRenderMeshSprite();

			Vector2[] uvs = mesh.uv;
			uvs[0].x = uvRect.x;
			uvs[0].y = uvRect.y;

			uvs[1].x = uvRect.width;
			uvs[1].y = uvRect.y;

			uvs[2].x = uvRect.width;
			uvs[2].y = uvRect.height;

			uvs[3].x = uvRect.x;
			uvs[3].y = uvRect.height;

			mesh.uv = uvs;

			meshFilter.mesh = mesh;
		}
	}

	// Lighting Sprite Renderer
	public Mesh preRenderMesh = null;
	public Mesh GetRenderMeshSprite() {
		if (preRenderMesh == null) {
			Mesh mesh = new Mesh();

			mesh.vertices = new Vector3[]{new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1)};
			mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
			mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

			preRenderMesh = mesh;
		}
		return(preRenderMesh);
	}

	// Lighting Source
	public Mesh preRenderMesh2 = null;
	public Mesh GetRenderMeshLight() {
		if (preRenderMesh2 == null) {
			Mesh mesh = new Mesh();

			mesh.vertices = new Vector3[]{new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1)};
			mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
			mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

			preRenderMesh2 = mesh;
		}
		return(preRenderMesh2);
	}
}
