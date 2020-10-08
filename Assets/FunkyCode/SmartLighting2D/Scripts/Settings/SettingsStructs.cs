using UnityEngine;

namespace LightingSettings {

	[System.Serializable]
	public class BufferPresetList {
		public BufferPreset[] list = new BufferPreset[1];

		public string[] GetBufferLayers() {
			string[] layers = new string[list.Length];

			for(int i = 0; i < list.Length; i++) {
				if (list[i].name.Length > 0) {
					layers[i] = list[i].name;
				} else {
					layers[i] = "Preset (Id: " + (i + 1) + ")";
				}
				
			}

			return(layers);
		}
	}
	
	[System.Serializable]
	public class PresetLayers {
		public LightingLayer[] list = new LightingLayer[1];

		private bool[] listArray = new bool[10];

		public bool[] GetLayersArray() {
			for(int i = 0; i < 10; i++) {
				listArray[i] = false;
			}

            foreach(LightingLayer layer in list) {
                int id = (int)layer;

                listArray[id] = true;
            }

            return(listArray);
        }
	}
    
    [System.Serializable]
	public class BufferPreset {
		public string name = "Default";

		public SortingLayer sortingLayer = new SortingLayer();

		public Color darknessColor = new Color(0, 0, 0, 1);
		public float lightingResolution = 1f;

		public PresetLayers dayLayers = new PresetLayers();
		public PresetLayers nightLayers = new PresetLayers();

		public BufferPreset (int id) {
			name = "Preset (Id: " + (id + 1) + ")";
		}
	}

	[System.Serializable]
	public class QualitySettings {
		public bool HDR = true;
		public ShadowQuality highQualityShadows = ShadowQuality.Detailed;
		public UpdateMethod updateMethod = UpdateMethod.LateUpadte;

		public enum UpdateMethod {
			LateUpadte,
			OnPreCull
		}

		public enum ShadowQuality {
			Default,
			Detailed,
			Soft
		}
	}

	[System.Serializable]
	public class Layers {
		public LayersList lightLayers = new LayersList();
		public LayersList nightLayers = new LayersList();
		public LayersList dayLayers = new LayersList();

		public Layers() {
			lightLayers.names[0] = "Light Layer 1";

			nightLayers.names[0] = "Night Layer 1";

			dayLayers.names[0] = "Day Layer 1";
		}
	}

	[System.Serializable]
	public class LayersList {
		public string[] names = new string[1];

		public string[] GetNames() {
			string[] layers = new string[names.Length + 1];

			layers[0] = "Default";

			for(int i = 0; i < names.Length; i++) {
				layers[i + 1] = names[i];
			}

			return(layers);
		}
	}

	[System.Serializable]
	public class FogOfWar {
		public bool enabled = false;

		public int bufferID = 0;

		[Range(0, 1)]
		public float resolution = 1;

		public SortingLayer sortingLayer = new SortingLayer();
	}

	[System.Serializable]
	public class EditorViewSettings {
		public bool enable = true;
	}

	[System.Serializable]
	public class DayLightingSettings {
		public bool enable = true;

		[Range(0, 1)]
		public float alpha = 1;

		[Range(0, 360)]
		public float direction = 270;

		[Range(0, 10)]
		public float height = 1;

		public Softness softness = new Softness();

		public BumpMap bumpMap = new BumpMap();

		[System.Serializable]
		public class Softness {
			public bool enable = true;
			public float intensity = 0.5f;
		}


		// Is this only bumpmap settings?
		[System.Serializable]
		public class BumpMap {
			[Range(0, 5)]
			public float height = 1;

			[Range(0, 5)]
			public float strength = 1;
		}
	}

	[System.Serializable]
	public class SortingLayer {
		public string Name;
		public int Order;

		public void ApplyToMeshRenderer(MeshRenderer meshRenderer) {
			if (meshRenderer == null) {
				return;
			}
			
			if (meshRenderer.sortingLayerName != Name) {
				meshRenderer.sortingLayerName = Name;
			}

			if (meshRenderer.sortingOrder != Order) {
				meshRenderer.sortingOrder = Order;
			}
		}
	}

	[System.Serializable]
	public class AtlasSettings {
		public static string[] SpriteAtlasSizeArray = new string[]{"2048", "1024", "512", "256"};

		public bool lightingSpriteAtlas = false;
		//public SpriteAtlasSize spriteAtlasSize = SpriteAtlasSize.px1024;

		public int spriteAtlasPreloadFoldersCount = 0;
		public string[] spriteAtlasPreloadFolders = new string[1];
	}

	[System.Serializable]
	public class LightingSourceSettings {
		public static string[] LightingSourceTextureSizeArray = new string[]{"Custom", "2048", "1024", "512", "256", "128"};

		public LightingSourceTextureSize fixedLightTextureSize = LightingSourceTextureSize.px2048;
		public int lightingBufferPreloadCount = 0;
	}

	[System.Serializable]
	public class AdditiveMode {
		public bool enable = false;

		[Range(0, 1)]
		public float alpha = 0.5f;

		public LightingSettings.SortingLayer sortingLayer = new LightingSettings.SortingLayer();
	}

	
	[System.Serializable]
	public class NormalMapMode {
		public NormalMapType type = NormalMapType.PixelToLight;
		
		public NormalMapTextureType textureType = NormalMapTextureType.Texture;
		
		public Texture texture;
		public Sprite sprite;

		public Texture GetBumpTexture() {
			switch(textureType) {
				case NormalMapTextureType.Sprite:
					if (sprite == null) {
						return(null);
					}

					return(sprite.texture);

				case NormalMapTextureType.Texture:
					return(texture);
			}
			
			return(null);
		}
	}

	[System.Serializable]
	public class DayNormalMapMode {
		public NormalMapTextureType textureType = NormalMapTextureType.Texture;
		
		public Texture texture;
		public Sprite sprite;

		public Texture GetBumpTexture() {
			switch(textureType) {
				case NormalMapTextureType.Sprite:
					if (sprite == null) {
						return(null);
					}

					return(sprite.texture);

				case NormalMapTextureType.Texture:
					return(texture);
			}
			
			return(null);
		}
	}

	[System.Serializable]
	public class GlowMode {
		public bool enable = false;

		[Range(1, 10)]
		public int glowSize = 1;

		[Range(1, 10)]
		public int glowIterations = 1;
	}
	
}
