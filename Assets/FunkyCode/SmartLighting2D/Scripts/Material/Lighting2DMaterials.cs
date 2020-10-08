using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Lighting2DMaterials {
	private Sprite penumbraSprite;
	private Sprite atlasPenumbraSprite;

	private Sprite whiteMaskSprite;
	private Sprite atlasWhiteMaskSprite;

	private Sprite blackMaskSprite;
	private Sprite atlasBlackMaskSprite;

	private LightingMaterial occlusionEdge = null;
	private LightingMaterial occlusionBlur = null;
	private LightingMaterial shadowBlur = null;
	private LightingMaterial additive = null;
	private LightingMaterial multiply = null;
	private LightingMaterial alphablend = null;

	private LightingMaterial whiteSprite = null;



	private LightingMaterial normalPixelToLightSprite = null;
	private LightingMaterial normalObjectToLightSprite = null;

	private LightingMaterial bumpedDaySprite = null;

	private LightingMaterial atlasMaterial = null;

	public bool hdr = false;
	private bool initialized = false;

	public Sprite GetPenumbraSprite() {
		if (penumbraSprite == null) {
			penumbraSprite = Resources.Load<Sprite>("textures/penumbra"); 
		}
		return(penumbraSprite);
	}

	public Sprite GetAtlasPenumbraSprite() {
		if (atlasPenumbraSprite == null) {
			atlasPenumbraSprite = AtlasSystem.Manager.RequestSprite(GetPenumbraSprite(), AtlasSystem.Request.Type.BlackMask);
		}
		return(atlasPenumbraSprite);
	}

	public Sprite GetBlackMaskSprite() {
		if (blackMaskSprite == null) {
			blackMaskSprite = Resources.Load<Sprite>("textures/black"); 
		}
		return(blackMaskSprite);
	}

	public Sprite GetWhiteMaskSprite() {
		if (whiteMaskSprite == null) {
			whiteMaskSprite = Resources.Load<Sprite>("textures/white"); 
		}
		return(whiteMaskSprite);
	}

	public Sprite GetAtlasWhiteMaskSprite() {
		if (atlasWhiteMaskSprite == null) {
			atlasWhiteMaskSprite = AtlasSystem.Manager.RequestSprite(GetWhiteMaskSprite(), AtlasSystem.Request.Type.WhiteMask);
		}
		return(atlasWhiteMaskSprite);
	}

	public Sprite GetAtlasBlackMaskSprite() {
		if (atlasBlackMaskSprite == null) {
			atlasBlackMaskSprite = AtlasSystem.Manager.RequestSprite(GetBlackMaskSprite(), AtlasSystem.Request.Type.Normal);
		}
		return(atlasBlackMaskSprite);
	}

	public bool Initialize(bool allowHDR) {
		if (initialized == true) {
			if (allowHDR == hdr) {
				return(false);
			}
		}

		hdr = allowHDR;

		Reset();

		initialized = true;

		GetPenumbraSprite();
		GetAtlasPenumbraSprite();

		GetWhiteMaskSprite();
		GetBlackMaskSprite();

		GetAtlasWhiteMaskSprite();
		GetAtlasBlackMaskSprite();

		GetAdditive();
		GetOcclusionBlur();
		GetOcclusionEdge();
		GetShadowBlur();

		GetWhiteSprite();

		GetNormalMapSpritePixelToLight();
		
		GetBumpedDaySprite();

		GetAtlasMaterial();

		return(true);
	}

	public void Reset() {
		initialized = false; // is it the best way?
	
		penumbraSprite = null;
		atlasPenumbraSprite = null;

		whiteMaskSprite = null;
		atlasWhiteMaskSprite = null;

		blackMaskSprite = null;
		atlasBlackMaskSprite = null;

		occlusionEdge = null;
		occlusionBlur = null;
		shadowBlur = null;
		additive = null;
		multiply = null;
		alphablend = null;

		whiteSprite = null;

		atlasMaterial = null;
	}

	public Material GetAtlasMaterial() {
		if (atlasMaterial == null || atlasMaterial.Get() == null) {
			atlasMaterial = LightingMaterial.Load(Max2D.shaderPath + "Particles/Alpha Blended");
		}
		
		atlasMaterial.SetTexture(AtlasSystem.Manager.GetAtlasPage().GetTexture());

		return(atlasMaterial.Get());
	}
	
	public Material GetAdditive() {
		if (additive == null || additive.Get() == null) {
			additive = LightingMaterial.Load(Max2D.shaderPath + "Particles/Additive");
		}
		return(additive.Get());
	}

	public Material GetMultiply() {
		if (multiply == null || multiply.Get() == null) {
			if (hdr == true) {
				multiply = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
			} else {
				multiply = LightingMaterial.Load(Max2D.shaderPath + "Particles/Multiply");
			}
		}
		return(multiply.Get());
	}

	public Material GetAlphaBlend() {
		if (alphablend == null || alphablend.Get() == null) {
			alphablend = LightingMaterial.Load(Max2D.shaderPath + "Particles/Alpha Blended");

			alphablend.SetTexture("textures/white");
		}
		return(alphablend.Get());
	}

	public Material GetOcclusionEdge() {
		if (occlusionEdge == null || occlusionEdge.Get() == null) {
			if (hdr == true) {
				occlusionEdge = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
			} else {
				occlusionEdge = LightingMaterial.Load(Max2D.shaderPath + "Particles/Multiply");
			}
			
			occlusionEdge.SetTexture("textures/occlusionedge");
		}
		return(occlusionEdge.Get());
	}

	public Material GetShadowBlur() {
		if (shadowBlur == null || shadowBlur.Get() == null) {
			if (hdr == true) {
				shadowBlur = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
			} else {
				shadowBlur = LightingMaterial.Load(Max2D.shaderPath + "Particles/Multiply");
			}
			
			shadowBlur.SetTexture("textures/shadowblur");
		}
		return(shadowBlur.Get());
	}

	public Material GetOcclusionBlur() {
		if (occlusionBlur == null || occlusionBlur.Get() == null) {
			if (hdr == true) {
				occlusionBlur = LightingMaterial.Load("SmartLighting2D/Multiply HDR");
			} else {
				occlusionBlur = LightingMaterial.Load(Max2D.shaderPath + "Particles/Multiply");
			}
			
			occlusionBlur.SetTexture("textures/occlussionblur");
		}
		return(occlusionBlur.Get());
	}

	public Material GetWhiteSprite() {
		if (whiteSprite == null || whiteSprite.Get() == null) {
			whiteSprite = LightingMaterial.Load("SmartLighting2D/SpriteWhite");
		}
		return(whiteSprite.Get());
	}


	public Material GetNormalMapSpritePixelToLight() {
		if (normalPixelToLightSprite == null || normalPixelToLightSprite.Get() == null) {
			normalPixelToLightSprite = LightingMaterial.Load("SmartLighting2D/NormalMapPixelToLight");
		}
		return(normalPixelToLightSprite.Get());
	}

	public Material GetNormalMapSpriteObjectToLight() {
		if (normalObjectToLightSprite== null || normalObjectToLightSprite.Get() == null) {
			normalObjectToLightSprite = LightingMaterial.Load("SmartLighting2D/NormalMapObjectToLight");
		}
		return(normalObjectToLightSprite.Get());
	}

	public Material GetBumpedDaySprite() {
		if (bumpedDaySprite == null || bumpedDaySprite.Get() == null) {
			bumpedDaySprite = LightingMaterial.Load("SmartLighting2D/DayBump");
		}
		return(bumpedDaySprite.Get());
	}


}