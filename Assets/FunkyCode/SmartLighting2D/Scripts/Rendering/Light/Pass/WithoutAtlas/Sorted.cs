using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {
        
    public class Sorted {

        public static void Draw(Rendering.Light.Pass pass) {
            for(int i = 0; i < pass.sortPass.sortList.count; i ++) {
                pass.sortPass.sortObject = pass.sortPass.sortList.list[i];

                switch (pass.sortPass.sortObject.type) {
                    case Sorting.SortObject.Type.Collider:
                        DrawCollider(pass);
                    break;

                    case Sorting.SortObject.Type.Tile:
                        DrawTile(pass);
                    break;

                }
            }
        }

        public static void DrawCollider(Rendering.Light.Pass pass) {
            LightingCollider2D collider = (LightingCollider2D)pass.sortPass.sortObject.lightObject;

            if ((int)collider.lightingCollisionLayer == pass.layerID && pass.drawShadows) {	
                // Shadows
                switch(collider.mainShape.colliderType) {
                    case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                    case LightingCollider2D.ColliderType.Collider2D:
                    case LightingCollider2D.ColliderType.CompositeCollider2D:
                    case LightingCollider2D.ColliderType.MeshRenderer:
                    case LightingCollider2D.ColliderType.SkinnedMeshRenderer:
                    
                        Lighting2D.materials.GetAtlasMaterial().SetPass(0);
                        GL.Begin(GL.TRIANGLES);

                            Light.Shadow.Shape.Draw(pass.buffer, collider);

                        GL.End();

                    break;
                }
            }

            // Masking
            if ((int)collider.lightingMaskLayer == pass.layerID && pass.drawMask) {
                pass.materialWhite.color = Color.white;

                switch(collider.mainShape.maskType) {
                    case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                    case LightingCollider2D.MaskType.Collider2D:
                    case LightingCollider2D.MaskType.CompositeCollider2D:
                    
                        pass.materialWhite.SetPass(0);
                        
                        GL.Begin(GL.TRIANGLES);
                            Light.Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                        GL.End();

                    break;

                    case LightingCollider2D.MaskType.Sprite:

                        SpriteRenderer2D.Mask(pass.buffer, collider, pass.materialWhite, pass.layer, pass.z);
                        
                        break;

                    case LightingCollider2D.MaskType.BumpedSprite:

                        if (collider.normalMapMode.type == NormalMapType.ObjectToLight) {
                            SpriteRenderer2D.MaskNormalMap(pass.buffer, collider, pass.materialNormalMap_ObjectToLight, pass.layer, pass.z);
                        } else {
                            SpriteRenderer2D.MaskNormalMap(pass.buffer, collider, pass.materialNormalMap_PixelToLight, pass.layer, pass.z);
                        }
                        
                        break;

                    case LightingCollider2D.MaskType.MeshRenderer:
                        pass.materialWhite.SetPass(0);
                        
                        GL.Begin(GL.TRIANGLES);
                            Mesh.Mask(pass.buffer, collider, pass.materialWhite, pass.layer, pass.z);
                        GL.End();
                    break;

                    case LightingCollider2D.MaskType.SkinnedMeshRenderer:
                        pass.materialWhite.SetPass(0);
            
                        GL.Begin(GL.TRIANGLES);
                            SkinnedMesh.Mask(pass.buffer, collider, pass.materialWhite, pass.layer, pass.z);
                        GL.End();
                    break;
                }
            }
        }

        static public void DrawTile(Rendering.Light.Pass pass) {
            #if UNITY_2017_4_OR_NEWER

                LightingTile tile = (LightingTile)pass.sortPass.sortObject.lightObject;

                Lighting2D.materials.GetAtlasMaterial().SetPass(0);
                                    
                GL.Begin(GL.TRIANGLES);

                    if ((int)pass.sortPass.sortObject.tilemap.lightingCollisionLayer == pass.layerID && pass.drawShadows) {	
                        Light.Shadow.Tile.Draw(pass.buffer, tile, pass.sortPass.sortObject.position, pass.sortPass.sortObject.tilemap);
                    }

                GL.End();  

                // sprite mask, but what about shape mask?
                if ((int)pass.sortPass.sortObject.tilemap.lightingMaskLayer == pass.layerID && pass.drawMask) {
                    Tile.MaskSprite(pass.buffer, tile, pass.layer, pass.materialWhite, pass.sortPass.sortObject.position + ShadowEngine.lightOffset, pass.sortPass.sortObject.tilemap, pass.lightSizeSquared, pass.z);
                }    

             #endif     
        }
    }
}