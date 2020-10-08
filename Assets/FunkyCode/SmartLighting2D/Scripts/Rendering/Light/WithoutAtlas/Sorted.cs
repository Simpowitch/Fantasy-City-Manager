using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {
        
    public class Sorted {
        
        public static void Draw(Rendering.Light.Sorted.Pass pass) {
            for(int i = 0; i < pass.list.count; i ++) {
                pass.depth = pass.list.list[i];

                switch (pass.depth.type) {
                    case ColliderDepth.Type.Collider:
                        if ((int)pass.depth.collider.lightingCollisionLayer == pass.layerID && pass.drawShadows) {	
                            // Shadows
                            switch(pass.depth.collider.shape.colliderType) {
                                case LightingCollider2D.ColliderType.Collider:
                                case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                                case LightingCollider2D.ColliderType.Mesh:
                                case LightingCollider2D.ColliderType.SkinnedMesh:
                                
                                Lighting2D.materials.GetAtlasMaterial().SetPass(0);
                                GL.Begin(GL.TRIANGLES);

                                    Light.Shadow.Shape.Draw(pass.buffer, pass.depth.collider, pass.lightSizeSquared, pass.z);

                                GL.End();

                                break;
                            }
                        }

                        // Masking
                        if ((int)pass.depth.collider.lightingMaskLayer == pass.layerID && pass.drawMask) {
                            pass.materialWhite.color = Color.white;

                            switch(pass.depth.collider.shape.maskType) {
                                case LightingCollider2D.MaskType.Collider:
                                case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                                    pass.materialWhite.SetPass(0);
                                   
                                    GL.Begin(GL.TRIANGLES);
                                        Light.Shape.Mask(pass.buffer, pass.depth.collider, pass.layer, pass.z);
                                    GL.End();

                                break;

                                case LightingCollider2D.MaskType.Sprite:

                                    SpriteRenderer2D.Mask(pass.buffer, pass.depth.collider, pass.materialWhite, pass.layer, pass.z);
                                    
                                    break;

                                case LightingCollider2D.MaskType.BumpedSprite:

                                    if (pass.depth.collider.normalMapMode.type == NormalMapType.ObjectToLight) {
                                        SpriteRenderer2D.MaskNormalMap(pass.buffer, pass.depth.collider, pass.materialNormalMap_ObjectToLight, pass.layer, pass.z);
                                    } else {
                                        SpriteRenderer2D.MaskNormalMap(pass.buffer, pass.depth.collider, pass.materialNormalMap_PixelToLight, pass.layer, pass.z);
                                    }
                                   
                                    break;

                                case LightingCollider2D.MaskType.Mesh:
                                    pass.materialWhite.SetPass(0);
                                    
                                    GL.Begin(GL.TRIANGLES);
                                        Mesh.Mask(pass.buffer, pass.depth.collider, pass.materialWhite, pass.layer, pass.z);
                                    GL.End();
                                break;

                                case LightingCollider2D.MaskType.SkinnedMesh:
                                    pass.materialWhite.SetPass(0);
                        
                                    GL.Begin(GL.TRIANGLES);
                                        SkinnedMesh.Mask(pass.buffer, pass.depth.collider, pass.materialWhite, pass.layer, pass.z);
                                    GL.End();
                                break;
                            }
                        }

                    break;

                    #if UNITY_2017_4_OR_NEWER

                        case ColliderDepth.Type.Tile:
                            Lighting2D.materials.GetAtlasMaterial().SetPass(0);
                                
                            GL.Begin(GL.TRIANGLES);

                                if ((int)pass.depth.tilemap.lightingCollisionLayer == pass.layerID && pass.drawShadows) {	
                                    Light.Shadow.Tile.Draw(pass.buffer, pass.depth.tile, pass.depth.polyOffset, pass.depth.tilemap, pass.lightSizeSquared, pass.z);
                                }

                            GL.End();  

                            // sprite mask, but what about shape mask?
                            if ((int)pass.depth.tilemap.lightingMaskLayer == pass.layerID && pass.drawMask) {
                               Tile.MaskSprite(pass.buffer, pass.depth.tile, pass.layer, pass.materialWhite, pass.depth.polyOffset, pass.depth.tilemap, pass.lightSizeSquared, pass.z);
                            } 
                        
                        break;

                    #endif
                }
            }
        }
    }
}