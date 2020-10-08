using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class FogOfWarSprite : MonoBehaviour {
    public static List<FogOfWarSprite> list = new List<FogOfWarSprite>();

    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private Material material;

    public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

    public Sprite GetSprite() {
        spriteRenderer.enabled = false;
        return(spriteRenderer.sprite);
    }

    public SpriteRenderer GetSpriteRenderer() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        material = spriteRenderer.sharedMaterial;
        return(spriteRenderer);
    }

    public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

    static public List<FogOfWarSprite> GetList() {
		return(list);
	}

    void Start() {
    
        
    }
}
