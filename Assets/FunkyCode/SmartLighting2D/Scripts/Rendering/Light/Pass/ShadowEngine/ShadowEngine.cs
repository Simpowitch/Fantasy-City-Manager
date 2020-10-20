using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShadowEngine {
    public static LightingSource2D light;
	public static Vector2 lightOffset = Vector2.zero;
	public static float lightSize = 0;
	public static bool lightDrawAbove = false;

	public static Vector2 objectOffset = Vector2.zero;

	public static bool perpendicularIntersection;
	public static LightingLayer effectLayer = LightingLayer.Layer1;

    // Layer Effect
    public static List<List<Polygon2D>> effectPolygons = new List<List<Polygon2D>>();

	// public static float shadowDistance;
	public static float shadowZ = 0;

	public static int drawMode = 0;
	
	// Legacy Shadows
	static public int shadowIterations = 1;

    public static void Draw(LightingBuffer2D buffer, List<Polygon2D> polygons, Vector2 scale, float height) {
		switch(ShadowEngine.drawMode) {
            case 0:
				Rendering.Light.Shadow.Legacy.Draw(buffer, polygons, scale, height);
			break;
		
			case 1:
				Rendering.Light.Shadow.Soft.Draw(buffer, polygons, scale);
			break;

            case 2:
				Rendering.Light.Shadow.PerpendicularIntersection.Draw(buffer, polygons, scale, height);
			break;

		}
	}

	public static void SetPass(LightingSource2D lightObject, LayerSetting layer) {
        light = lightObject;
		lightSize = Mathf.Sqrt(light.size * light.size + light.size * light.size);
		lightOffset = -light.transform2D.position;

		effectLayer = layer.shadowEffectLayer;

		objectOffset = Vector2.zero;

		shadowIterations = Lighting2D.commonSettings.shadowIterations;

        effectPolygons.Clear();
            
		if (layer.shadowEffect == LightingLayerShadowEffect.Projected) {
			drawMode = 2;

			GenerateEffectLayers();

		} else if (layer.shadowEffect == LightingLayerShadowEffect.Soft) {
			drawMode = 1;
			
		} else {
			drawMode = 0;
		}

        
	}

    public static void GenerateEffectLayers() {
        int layerID = (int)ShadowEngine.effectLayer;

        foreach(LightingCollider2D c in LightingCollider2D.GetEffectList((layerID))) {
            List<Polygon2D> polygons = c.mainShape.GetPolygonsWorld();

            if (polygons == null) {
                continue;
            }
  
            if (c.InLightSource(light.Buffer)) {
                effectPolygons.Add(polygons);
            }
        }
    }
	
	public static void Prepare(LightingSource2D light) {
		FillWhite.Calculate();
		Penumbra.Calculate();

		lightDrawAbove = light.whenInsideCollider == LightingSource2D.WhenInsideCollider.DrawAbove;
	}

	static public class Penumbra {
        static public Rect uvRect = new Rect();
        static public Vector2 size;
 
        static Sprite sprite = null;

        public static void Calculate() {
            LightingManager2D manager = LightingManager2D.Get();
            
            sprite = Lighting2D.materials.GetAtlasPenumbraSprite();

            if (sprite == null || sprite.texture == null) {
                return;
            }

            Rect spriteRect = sprite.textureRect;
            int atlasSize = AtlasSystem.Manager.GetAtlasPage().atlasSize / 2;

            uvRect.x = spriteRect.x / sprite.texture.width;
            uvRect.y = spriteRect.y / sprite.texture.height;

            size.x = ((float)spriteRect.width) / sprite.texture.width;
            size.y = ((float)spriteRect.height) / sprite.texture.height;

            uvRect.width = spriteRect.width / 2 / sprite.texture.width;
            uvRect.height = spriteRect.height / 2 / sprite.texture.height;

            uvRect.width += uvRect.x;
            uvRect.height += uvRect.y;

            uvRect.x += 1f / atlasSize;
            uvRect.y += 1f / atlasSize;
            uvRect.width -= 1f / atlasSize;
            uvRect.height -= 1f / atlasSize;
        }
    }

	public class FillWhite {
        static public Rect uvRect = new Rect();

        static public void Calculate() {
            LightingManager2D manager = LightingManager2D.Get();
            
            Sprite fillSprite = Lighting2D.materials.GetAtlasWhiteMaskSprite();

            if (fillSprite != null) {
                Rect spriteRect = fillSprite.textureRect;

                uvRect.x = spriteRect.x / fillSprite.texture.width;
                uvRect.y = spriteRect.y / fillSprite.texture.height;
                uvRect.width = spriteRect.width / fillSprite.texture.width;
                uvRect.height = spriteRect.height / fillSprite.texture.height;

                uvRect.x += uvRect.width / 2;
                uvRect.y += uvRect.height / 2;
                
                Max2D.texCoord = new Vector2(uvRect.x, uvRect.y);
            }
        }
    }
	
	public class FillBlack {
        static public Rect uvRect = new Rect();
        
        static public void Calculate() {
            LightingManager2D manager = LightingManager2D.Get();
            
            Sprite fillSprite = Lighting2D.materials.GetAtlasBlackMaskSprite();
        
            if (fillSprite != null) {
                Rect spriteRect = fillSprite.textureRect;

                uvRect.x = spriteRect.x / fillSprite.texture.width;
                uvRect.y = spriteRect.y / fillSprite.texture.height;
                uvRect.width = spriteRect.width / fillSprite.texture.width;
                uvRect.height = spriteRect.height / fillSprite.texture.height;

                uvRect.x += uvRect.width / 2;
                uvRect.y += uvRect.height / 2;
                
                Max2D.texCoord = new Vector2(uvRect.x, uvRect.y);
            }
        }
    }
}