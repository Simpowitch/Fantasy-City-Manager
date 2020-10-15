
namespace LightingSettings {

	public enum RenderingMode {
		OnRender = 2,
		OnPostRender = 1,
		OnPreRender = 0	
	}

	public enum SpriteAtlasSize {
		px2048,
		px1024,
		px512,
		px256
	}

	public enum CoreAxis {
		XY,
		XZ
	}

	public enum LightingSourceTextureSize {
		Custom, 
		px2048, 
		px1024, 
		px512, 
		px256, 
		px128
	}
}