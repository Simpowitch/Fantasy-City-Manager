using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {

    public class NoSort {

        public static void Draw(Camera camera, Vector2 offset, float z, int layer) {
            // Draw Rooms
            foreach (LightingRoom2D id in LightingRoom2D.GetList()) {
                if ((int)id.nightLayer != layer) {
                    continue;
                }

                Room.Draw(id, camera, offset, z);
            }

            // Draw Tilemap Rooms
            #if UNITY_2017_4_OR_NEWER
            
                foreach (LightingTilemapRoom2D id in LightingTilemapRoom2D.GetList()) {
                    if ((int)id.nightLayer != layer) {
                        continue;
                    }
                    
                    TilemapRoom.Draw(id, camera, offset, z);
                }
            #endif

            // Draw Light Sprite
            List<LightingSpriteRenderer2D> spriteRendererList = LightingSpriteRenderer2D.GetList();
            for(int i = 0; i < spriteRendererList.Count; i++) {
                LightingSpriteRenderer2D id = spriteRendererList[i];

                if ((int)id.nightLayer != layer) {
                    continue;
                }

                SpriteRenderer2D.Draw(id, camera, offset, z);
            }

            // Draw Light Texture
            List<LightingTextureRenderer2D> textureRendererList= LightingTextureRenderer2D.GetList();
			for(int i = 0; i < textureRendererList.Count; i++) {
				LightingTextureRenderer2D id = textureRendererList[i];

				if ((int)id.nightLayer != layer) {
					continue;
				}

				TextureRenderer.Draw(id, camera, offset, z);
			}

            // Draw Light Particle Renderer
            List<LightingParticleRenderer2D> particleRendererList = LightingParticleRenderer2D.GetList();
			for(int i = 0; i < particleRendererList.Count; i++) {
				LightingParticleRenderer2D id = particleRendererList[i];

				if ((int)id.nightLayer != layer) {
					continue;
				}

				ParticleRenderer.Draw(id, camera, offset, z);
			}

            // Draw Light Source
            foreach (LightingSource2D id in LightingSource2D.GetList()) {
                if ((int)id.nightLayer != layer) {
                    continue;
                }

               Rendering.Night.LightSource.Draw(id, camera, offset, z);
            }
        }
    }
}