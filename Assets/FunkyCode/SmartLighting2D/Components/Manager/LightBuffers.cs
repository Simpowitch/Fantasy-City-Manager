using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class LightBuffers : LightingMonoBehaviour {
    static private LightBuffers instance;

    public void Awake() {
        //foreach(LightingBuffer2D buffer in Object.FindObjectsOfType(typeof(LightingBuffer2D))) {
		//	buffer.DestroySelf();
		//}

		LightingBuffer2D.Clear();
    }

	static public int GetCount() {
		return(LightingBuffer2D.list.Count);
	}

    static public LightBuffers Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightBuffers mainBuffer in Object.FindObjectsOfType(typeof(LightBuffers))) {
			instance = mainBuffer;
			return(instance);
		}

		GameObject gameObject = new GameObject ("Light Buffers");
		gameObject.transform.parent = LightingManager2D.Get().transform;

		instance = gameObject.AddComponent<LightBuffers> ();

		return(instance);
	}

    // Management
	static public LightingBuffer2D AddBuffer(int textureSize, LightingSource2D light) {
		Get();

        if (Lighting2D.Profile.qualitySettings.fixedLightTextureSize != LightingSettings.LightingSourceTextureSize.Custom) {
            textureSize = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.fixedLightTextureSize);
        }

		LightBuffers lightBuffers = LightBuffers.Get();
		
		LightingBuffer2D lightingBuffer2D = new LightingBuffer2D ();
		lightingBuffer2D.Initiate (textureSize);
		lightingBuffer2D.lightSource = light; // Unnecessary?
        
		if (light != null) {
			lightingBuffer2D.Free = false;
		} else {
			lightingBuffer2D.Free = true;
		}

		return(lightingBuffer2D);
	}

	static public LightingBuffer2D PullBuffer(int textureSize, LightingSource2D lightSource) {
		Get();

        if (Lighting2D.Profile.qualitySettings.fixedLightTextureSize != LightingSettings.LightingSourceTextureSize.Custom) {
            textureSize = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.fixedLightTextureSize);
        }

		foreach (LightingBuffer2D id in LightingBuffer2D.GetList()) {
			if (id.Free && id.renderTexture.width == textureSize) {
				id.lightSource = lightSource;
				id.Free = false;

				lightSource.ForceUpdate();
				
				return(id);
			}
		}
			
		return(AddBuffer(textureSize, lightSource));		
	}

    static public void FreeBuffer(LightingBuffer2D buffer) {
		
        if (buffer == null) {
            return;
        }

		if (buffer.lightSource != null) {
			buffer.lightSource.Buffer = null;

			buffer.lightSource = null;
		}

		buffer.updateNeeded = false;
        
        buffer.Free = true;
	}
}
