using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {

    public class NoSort  {

        public static void Draw(Rendering.Light.NoSort.Pass pass) {
            Lighting2D.materials.GetAtlasMaterial().SetPass(0);

            GL.Begin(GL.TRIANGLES);

            if (pass.drawShadows) {
                Shadows.Draw(pass);
            }

            if (pass.drawMask) {
               Mask.Draw(pass);
            }

            GL.End();

            DrawBatchedColliderMask(pass);
            DrawBatchedTilemapColliderMask(pass);
        }

        public class Shadows {
            static public void Draw(Rendering.Light.NoSort.Pass pass) {
                int colliderCount = pass.colliderList.Count;

                if (colliderCount > 0) {
                    for(int id = 0; id < colliderCount; id++) {
                        LightingCollider2D collider = pass.colliderList[id]; 

                        if ((int)collider.lightingCollisionLayer != pass.layerID) {
                            continue;
                        }

                        switch(collider.shape.colliderType) {
                            case LightingCollider2D.ColliderType.Collider:
                            case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                            case LightingCollider2D.ColliderType.Mesh:
                            case LightingCollider2D.ColliderType.SkinnedMesh:
                                Shadow.Shape.Draw(pass.buffer, collider, pass.lightSizeSquared, pass.z);
                            break;
                        }

                    }
                }

                #if UNITY_2017_4_OR_NEWER
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingCollisionLayer != pass.layerID) {
                            continue;
                        }

                        switch(pass.tilemapList[id].mapType) {
                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
                                Shadow.TilemapRectangle.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                            break;

                        }

                        if (pass.tilemapList[id].colliderType == LightingTilemapCollider2D.ColliderType.Collider) {
                            Shadow.TilemapCollider.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                        }

                    }
                #endif 

            }
        }

        public class Mask {
             static public void Draw(Rendering.Light.NoSort.Pass pass) {
                int colliderCount = pass.colliderList.Count;

                if (colliderCount > 0) {
                    for(int id = 0; id < colliderCount; id++) {
                        LightingCollider2D collider = pass.colliderList[id]; 
                        
                        if ((int)collider.lightingMaskLayer != pass.layerID) {
                            continue;
                        }

                        switch(collider.shape.maskType) {
                            case LightingCollider2D.MaskType.Collider:
                                Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                            break;

                            case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                                Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                            break;

                            case LightingCollider2D.MaskType.Sprite:
                                SpriteRenderer2D.Mask(pass.buffer, collider, pass.z);

                            break;
                        }
                    }
                }
                    
                #if UNITY_2017_4_OR_NEWER		
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingMaskLayer != pass.layerID) {
                            continue;
                        }

                        WithoutAtlas.TilemapRectangle.MaskShape(pass.buffer, pass.tilemapList[id], pass.z);

                        // ?
                        WithAtlas.TilemapRectangle.Sprite(pass.buffer, pass.tilemapList[id], pass.z);
                    }
                #endif
            } 
        }
       
        static public void DrawBatchedColliderMask(Rendering.Light.NoSort.Pass pass) {
            // Partialy Batched (Default Edition)
            if (pass.buffer.lightingAtlasBatches.colliderList.Count > 0) {
                PartiallyBatchedCollider batch;			

                for(int i = 0; i < pass.buffer.lightingAtlasBatches.colliderList.Count; i++) {
                    batch = pass.buffer.lightingAtlasBatches.colliderList[i];

                    WithoutAtlas.SpriteRenderer2D.Mask(pass.buffer, batch.collider, pass.materialWhite, pass.layer, pass.z);
                }

                pass.buffer.lightingAtlasBatches.colliderList.Clear();
            }
        }

        static public void DrawBatchedTilemapColliderMask(Rendering.Light.NoSort.Pass pass) {
            if (pass.buffer.lightingAtlasBatches.tilemapList.Count > 0) {
                PartiallyBatchedTilemap batch;

                for(int i = 0; i < pass.buffer.lightingAtlasBatches.tilemapList.Count; i++) {
                    batch = pass.buffer.lightingAtlasBatches.tilemapList[i];

                    pass.materialWhite.color = LayerSettingColor.Get(batch.polyOffset, pass.layer, MaskEffect.Lit);

                    pass.materialWhite.mainTexture = batch.virtualSpriteRenderer.sprite.texture;

                    Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(batch.tile.spriteMeshObject, pass.materialWhite, batch.virtualSpriteRenderer, batch.polyOffset, batch.tileSize, 0, pass.z);

                    pass.materialWhite.mainTexture = null;
                }
                
                pass.buffer.lightingAtlasBatches.tilemapList.Clear();
            }
        }
    }
}
