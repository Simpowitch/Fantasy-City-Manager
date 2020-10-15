using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering {

	public class FogOfWarBuffer {
		public class Check {
			static public void RenderTexture(FogOfWarBuffer2D buffer) {
                Vector2Int screen = buffer.GetScreen();

                if (screen.x > 0 && screen.y > 0) {
                    Camera camera = buffer.cameraSettings.GetCamera();

                    if (buffer.renderTexture == null || screen.x != buffer.renderTexture.width || screen.y != buffer.renderTexture.height) {

                        switch(camera.cameraType) {
                            case CameraType.Game:
                                buffer.SetUpRenderTexture();
                            
                            break;

                            case CameraType.SceneView:
                                // Scene view pixel rect is constantly changing (Unity Bug?)
                                int differenceX = Mathf.Abs(screen.x - buffer.renderTexture.width);
                                int differenceY = Mathf.Abs(screen.y - buffer.renderTexture.height);
                                
                                if (differenceX > 5 || differenceY > 5) {
                                    buffer.SetUpRenderTexture();
                                }
                            
                            break;

                        }
                    }
                }
            }
		}
		public static void LateUpdate(FogOfWarBuffer2D buffer) {

			if (buffer.CameraSettingsCheck() == false) {
				buffer.DestroySelf();
				return;
			}

			Camera camera = buffer.cameraSettings.GetCamera();

			if (camera == null) {
				return;
			}

			buffer.transform.position = new Vector3(0, 0, 0);
			buffer.transform.rotation = Quaternion.Euler(0, 0, 0);

			if (Lighting2D.fogOfWar.enabled) {
				buffer.updateNeeded = true;
			} else {
				buffer.updateNeeded = false;
			}

			if (Lighting2D.fogOfWar.enabled == false) {
				buffer.DestroySelf();

				return;
			}

		}

		public static void DrawOn(FogOfWarBuffer2D buffer) {
			if (Lighting2D.fogOfWar.enabled == false) {
				return;
			}

			switch(Lighting2D.renderingMode) {
				case RenderingMode.OnRender:
					FogOfWarRender.OnRender(buffer);
				break;

				case RenderingMode.OnPreRender:
					FogOfWarRender.PreRender(buffer);
				break;
			}
		}

		static public void Render(FogOfWarBuffer2D buffer) {
			Camera camera = buffer.cameraSettings.GetCamera();

			if (camera == null) {
				return;
			}

			float z = 0;
			Material material = null;

			Vector2 objectPosition = new Vector2();

			float sizeY = camera.orthographicSize;
			float sizeX = sizeY * ( (float)camera.pixelWidth / camera.pixelHeight );

			GL.PushMatrix();
			GL.LoadPixelMatrix( -sizeX, sizeX, -sizeY, sizeY );

			foreach(FogOfWarSprite sprite in FogOfWarSprite.GetList()) {
				objectPosition.x = sprite.transform.position.x - camera.transform.position.x;
				objectPosition.y = sprite.transform.position.y - camera.transform.position.y;

				SpriteRenderer spriteRenderer = sprite.GetSpriteRenderer();

				if (spriteRenderer == null || sprite.GetSprite() == null) {
					continue;
				}

				material = spriteRenderer.sharedMaterial;
				material.mainTexture = sprite.GetSprite().texture;

				material.color = spriteRenderer.color;

				Rendering.Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(sprite.spriteMeshObject, material, sprite.GetSpriteRenderer(), objectPosition, sprite.transform.lossyScale, sprite.transform.rotation.eulerAngles.z, z);			
			
				material.color = Color.white;
			}

			GL.PopMatrix();
		}
	}
}