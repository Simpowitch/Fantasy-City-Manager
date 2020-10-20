
using UnityEngine;

[System.Serializable]
public class LayerSetting {
	public LightingLayer layerID = LightingLayer.Layer1;
	public LightingLayerType type = LightingLayerType.ShadowAndMask;

	public LightingLayerSorting sorting = LightingLayerSorting.None;
	public LightingLayerSortingIgnore sortingIgnore = LightingLayerSortingIgnore.None;

	public LightingLayerShadowEffect shadowEffect = LightingLayerShadowEffect.Default;
	public LightingLayer shadowEffectLayer = LightingLayer.Layer1;

	public LightingLayerMaskEffect maskEffect = LightingLayerMaskEffect.AlwaysLit;
	public float maskEffectDistance = 1;

	public int GetLayerID() {
		int layer = (int)layerID;

		if (layer < 0) {
			return(-1);
		}

		return(layer);
	}
}

public class LayerSettingColor {
    public static Color Get(Vector2 position, LayerSetting layerSetting, MaskEffect maskEffect) {
		if (maskEffect == MaskEffect.Unlit) {
			return(Color.black);
		}
		
		if (layerSetting.maskEffect == LightingLayerMaskEffect.AboveLit) {
            return(LayerSettingsColorEffects.GetColor(position, layerSetting));
        } else if (layerSetting.maskEffect == LightingLayerMaskEffect.NeverLit) {
			return(Color.black);
		} else {
            return(Color.white);
        }
	}
}

public class LayerSettingsColorEffects {
	public static Color GetColor(Vector2 position, LayerSetting layerSetting) {
		float distance = layerSetting.maskEffectDistance;

		float pos, c;
		
		if (distance > 0) {
			pos = (position.y - distance / 2) / distance;

			c = (pos  + 1);
		} else {
			pos = position.y;

			if (pos < 0) {
				c = 0;
			} else {
				c = 1;
			}
		}
		


		if (c < 0) {
			c = 0;
		}

		if (c > 1) {
			c = 1;
		}

		return(new Color(c, c, c, 1));
	}
}