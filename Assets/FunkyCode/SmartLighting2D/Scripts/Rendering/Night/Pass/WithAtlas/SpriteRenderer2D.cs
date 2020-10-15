using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithAtlas {
	
    public class SpriteRenderer {

       static VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

        static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
            List<LightingSpriteRenderer2D> spriteRendererList = LightingSpriteRenderer2D.GetList();

            Material material = Lighting2D.materials.GetAdditive();
            material.SetColor ("_TintColor", Color.white);
            material.mainTexture = AtlasSystem.Manager.GetAtlasPage().GetTexture();

            material.SetPass (0);

            GL.Begin (GL.TRIANGLES);
    
            for(int i = 0; i < spriteRendererList.Count; i++) {
                LightingSpriteRenderer2D id = spriteRendererList[i];

                id = spriteRendererList[i];

                if ((int)id.nightLayer != nightLayer) {
                    continue;
                }

                if (id.type != LightingSpriteRenderer2D.Type.Particle) {
                    continue;
                }

                if (id.GetSprite() == null) {
                    continue;
                }

                if (id.InCamera(camera) == false) {
                    continue;
                }

                Vector2 position = id.transform.position;

                Vector2 scale = id.transform.lossyScale;
                scale.x *= id.transformOffset.offsetScale.x;
                scale.y *= id.transformOffset.offsetScale.y;

                float rot = id.transformOffset.offsetRotation;
                if (id.transformOffset.applyTransformRotation) {
                    rot += id.transform.rotation.eulerAngles.z;
                }

                Color color = id.color;

                GL.Color(color);

                spriteRenderer.sprite = AtlasSystem.Manager.RequestSprite(id.GetSprite(), AtlasSystem.Request.Type.Normal);
                
                Rendering.Universal.WithAtlas.Sprite.Draw(spriteRenderer, offset + position + id.transformOffset.offsetPosition, scale, rot, z);
            }

            GL.End();

            material = Lighting2D.materials.GetAtlasMaterial();
            material.SetPass (0);

            GL.Begin (GL.TRIANGLES);
            
            for(int i = 0; i < spriteRendererList.Count; i++) {
                LightingSpriteRenderer2D id = spriteRendererList[i];

                if (id.type == LightingSpriteRenderer2D.Type.Particle) {
                    continue;
                }

                if (id.GetSprite() == null) {
                    continue;
                }

                if (id.InCamera(camera) == false) {
                    continue;
                }

                Vector2 position = id.transform.position;

                Vector2 scale = id.transform.lossyScale;
                scale.x *= id.transformOffset.offsetScale.x;
                scale.y *= id.transformOffset.offsetScale.y;

                float rot = id.transformOffset.offsetRotation;
                if (id.transformOffset.applyTransformRotation) {
                    rot += id.transform.rotation.eulerAngles.z;
                }

                switch(id.type) {
                    case LightingSpriteRenderer2D.Type.Mask:
                        GL.Color(id.color);

                        spriteRenderer.sprite = AtlasSystem.Manager.RequestSprite(id.GetSprite(), AtlasSystem.Request.Type.WhiteMask);
                        
                        Rendering.Universal.WithAtlas.Sprite.Draw(spriteRenderer, offset + position + id.transformOffset.offsetPosition, scale, rot, z);
                        break;
                }
            }

            GL.End();
        }
    }
}