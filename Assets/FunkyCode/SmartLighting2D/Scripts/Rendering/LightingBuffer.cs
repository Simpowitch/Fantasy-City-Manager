using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering {
    public class LightingBuffer {

        static public void Render(LightingBuffer2D buffer) {
			float size = buffer.lightSource.size;

			Rendering.Light.FillWhite.Calculate();
			Rendering.Light.Penumbra.Calculate();
			Rendering.Light.ShadowSetup.Calculate(buffer);

			GL.PushMatrix();

			GL.LoadPixelMatrix( -size, size, -size, size );

			LayerSetting[] layerSettings = buffer.lightSource.layerSetting;

			if (layerSettings != null) {

				for (int layerID = 0; layerID < layerSettings.Length; layerID++) {
					LayerSetting layerSetting = layerSettings[layerID];

					if (layerSetting == null) {
						continue;
					}

					if (layerSetting.sorting == LightingLayerSorting.None) {
						Light.NoSort.Draw(buffer, layerSetting);
					} else {
						Light.Sorted.Draw(buffer, layerSetting);
					}
				}

			}
			
			Light.LightSource.Main.Draw(buffer);

			GL.PopMatrix();
		}

        static public void UpdateName(LightingBuffer2D buffer) {
            string freeString = "";

            if (buffer.Free) {
                freeString = "free";
            } else {
                freeString = "taken";
            }

            if (buffer.renderTexture != null) {
                    
                buffer.name = "Buffer (Id: " + (LightingBuffer2D.GetList().IndexOf(buffer) + 1) + ", Size: " + buffer.renderTexture.width + ", " + freeString + ")";

            } else {
                buffer.name = "Buffer (Id: " + (LightingBuffer2D.GetList().IndexOf(buffer) + 1) + ", No Texture, " + freeString + ")";

            }
           
            if (Lighting2D.commonSettings.HDR) {
                buffer.name = "HDR " + buffer.name;
            }
        }

        static public void InitializeRenderTexture(LightingBuffer2D buffer, int textureSize) {
            RenderTextureFormat format = RenderTextureFormat.Default;

            if (Lighting2D.commonSettings.HDR) {
                format = RenderTextureFormat.DefaultHDR;
            }
            
            buffer.renderTexture = new RenderTexture(textureSize, textureSize, 0, format);

            UpdateName(buffer);
        }

        static public void LateUpdate(LightingBuffer2D buffer) {
            #if UNITY_EDITOR
                // It does not seem to work!
                LightingManager2D manager = LightingManager2D.Get();

                if (manager != null) {
                    buffer.gameObject.layer = manager.gameObject.layer;
                } else {
                    buffer.gameObject.layer = 0;
                }
            #endif
        }

        static public void Update(LightingBuffer2D buffer) {
			buffer.transform.position = new Vector3(0, 0, 0);
			buffer.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
    }
}