using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Universal.WithoutAtlas {

	public class Sprite : Base {
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

		static  public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation, float z) {
			//Debug.Log(spriteRenderer.sprite.);

			//TextureImporterSettings textureSettings = new TextureImporterSettings();
     		//importer.ReadTextureSettings(textureSettings);

			FullRect.Draw(spriteMeshObject, material, spriteRenderer, position, scale,  rotation, z);
		}

		public class Tight {

		}

		public class FullRect {

			public class Simple {
				
				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation, float z) {
					virtualSpriteRenderer.Set(spriteRenderer);
					
					Draw(spriteMeshObject, material, virtualSpriteRenderer, position, scale, rotation, z);
				}

				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation, float z) {
					SpriteTransform spriteTransform = new SpriteTransform(spriteRenderer, position, scale, rotation);

					//Mesh mesh = spriteMeshObject.GetRectMesh(spriteTransform);
					
					//Vector3 matrixPosition = new Vector3(spriteTransform.position.x, spriteTransform.position.y, z);
					//Quaternion matrixRotation = Quaternion.Euler(0, 0, spriteTransform.rotation);
					//Vector3 matrixScale = new Vector3(spriteTransform.scale.x, spriteTransform.scale.y, 1);
					
					material.SetPass(0);

					//Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(matrixPosition, matrixRotation, matrixScale));

			
					WithoutAtlas.Texture.Draw(material, spriteTransform.position, spriteTransform.scale, spriteTransform.uv, rotation, z);

					GL.End ();
				
					// Does not work with Normal Mapping
					// WithoutAtlas.Texture.Draw(material, spriteTransform.position, spriteTransform.scale, spriteTransform.uv, rotation, z);
				}
			}

			public class Tiled {
				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation, float z) {
					Vector3 matrixPosition = new Vector3(pos.x, pos.y, z);
					Quaternion matrixRotation = Quaternion.Euler(0, 0, rotation);
					Vector3 matrixScale = new Vector3(size.x, size.y, 1);

					material.SetPass (0); 

					// Change to GL
					//Graphics.DrawMeshNow(spriteMeshObject.GetTiledMesh().GetMesh(spriteRenderer).mesh, Matrix4x4.TRS(matrixPosition, matrixRotation, matrixScale));

					GLExtended.DrawMesh(spriteMeshObject.GetTiledMesh().GetMesh(spriteRenderer), pos, size, rotation);

				
				}
			}

			static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation, float z) {
				if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {
					Tiled.Draw(spriteMeshObject, material, spriteRenderer, pos, size, rotation, z);
				} else {
					Simple.Draw(spriteMeshObject, material, spriteRenderer, pos, size, rotation, z);
				}
			}
		}	
    }
}