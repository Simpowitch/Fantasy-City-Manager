using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering {

    public class LightingMainBuffer {

        public class Check {
                    
            static public void RenderTexture(LightingMainBuffer2D buffer, Camera camera, BufferPreset bufferPreset) {
                float resolution = bufferPreset.lightingResolution;

                int width = (int)(camera.pixelRect.width * resolution);
                int height = (int)(camera.pixelRect.height * resolution);

                if (buffer.renderTexture == null || width != buffer.renderTexture.width || height != buffer.renderTexture.height) {
                    Rendering.LightingMainBuffer.InitializeRenderTexture(buffer);
                }
            }

            static public bool CameraSettings (LightingMainBuffer2D buffer) {
                LightingManager2D manager = LightingManager2D.Get();
                int settingsID = buffer.cameraSettings.id;

                if (settingsID >= manager.cameraSettings.Length) {
                    return(false);
                }

                CameraSettings cameraSetting = manager.cameraSettings[settingsID];

                if (cameraSetting.Equals(buffer.cameraSettings) == false) {
                    return(false);
                }

                buffer.cameraSettings.renderMode = cameraSetting.renderMode;

                return(true);
            }

        }

        public static void LateUpdate(LightingMainBuffer2D buffer) {
            BufferPreset bufferPreset = buffer.GetBufferPreset();

            if (bufferPreset == null) {
                buffer.DestroySelf();
                return;
            }

            if (Rendering.LightingMainBuffer.Check.CameraSettings(buffer) == false) {
                buffer.DestroySelf();
                return;
            }

            Camera camera = buffer.cameraSettings.GetCamera();

            if (camera == null) {
                return;
            }

            Rendering.LightingMainBuffer.Check.RenderTexture(buffer, camera, bufferPreset);
        }

        public static void DrawPost(LightingMainBuffer2D buffer) {
            if (Lighting2D.renderingMode != RenderingMode.OnPostRender) {
				return;
			}
			
			if (buffer.cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
				return;
			}

			LightingRender2D.PostRender(buffer);
        }

        public static void DrawOn(LightingMainBuffer2D buffer) {
			if (buffer.cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
				return;
			}
                
            switch(Lighting2D.renderingMode) {
                case RenderingMode.OnRender:
                    LightingRender2D.OnRender(buffer);
                break;

                case RenderingMode.OnPreRender:
                    LightingRender2D.PreRender(buffer);
                break;
            }
        }

        public static void Render(LightingMainBuffer2D buffer) {
            Camera camera = buffer.cameraSettings.GetCamera();
            if (camera == null) {
                return;
            }

            Vector2 offset = new Vector2(-camera.transform.position.x, -camera.transform.position.y);
            float z = 0; // buffer.transform.position.z

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, -camera.transform.rotation.eulerAngles.z), Vector3.one);
            float sizeY = camera.orthographicSize;
            float sizeX = sizeY * ( (float)camera.pixelWidth / camera.pixelHeight );

            GL.LoadPixelMatrix( -sizeX, sizeX, -sizeY, sizeY );
            GL.MultMatrix(matrix);

            GL.PushMatrix();
          
            BufferPreset bufferPreset = buffer.GetBufferPreset();
            
            if (Lighting2D.dayLightingSettings.enable) {
                Rendering.Day.Main.Draw(camera, offset, z, bufferPreset);
            }

            Rendering.Night.Main.Draw(camera, offset, z, bufferPreset);

            GL.PopMatrix();
        }

        static public void InitializeRenderTexture(LightingMainBuffer2D buffer) {
            BufferPreset bufferPreset = buffer.GetBufferPreset();

            if (bufferPreset == null) {
                return;
            }

            Camera camera = buffer.cameraSettings.GetCamera();

            if (camera == null) {
                return;
            }
            
            float resolution = bufferPreset.lightingResolution;
            
            int screenWidth = (int)(camera.pixelRect.width * resolution);
            int screenHeight = (int)(camera.pixelRect.height * resolution);

            if (screenWidth > 0 && screenHeight > 0) {
                string idName = "";

                int bufferID = buffer.cameraSettings.bufferID;
                
                if (bufferID < Lighting2D.bufferPresets.Length) {
                    idName = Lighting2D.bufferPresets[bufferID].name + ", ";
                }
                
                RenderTextureFormat format = RenderTextureFormat.Default;
                if (Lighting2D.commonSettings.HDR == false) {
                    buffer.name = "Camera Buffer (" + idName +"Id: " + (bufferID  + 1) + ", Camera: " + camera.name + " )";
        
                    format = RenderTextureFormat.Default;
                } else {
                    buffer.name = "HDR Camera Buffer (" + idName +"Id: " + (bufferID + 1) + ", Camera: " + camera.name + " )";
                    format = RenderTextureFormat.DefaultHDR;
                }

                buffer.renderTexture = new RenderTexture (screenWidth, screenHeight, 0, format);
                buffer.renderTexture.Create ();
            }
        }
    }
}
