using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class NoSort {

        public static void Draw(Rendering.Light.NoSort.Pass pass) {
            if (pass.drawShadows) {
                Shadow.Draw(pass);
            }

            if (pass.drawMask) {
                Mask.Draw(pass);
            }
        }

        public class Shadow {

            public static void Draw(Rendering.Light.NoSort.Pass pass) {
                Material material = Lighting2D.materials.GetAtlasMaterial();
                material.color = Color.white;

                material.SetPass(0);

                GL.Begin(GL.TRIANGLES);

                DrawCollider(pass);
                DrawTilemapCollider(pass);
        
                GL.End();
            }

            public static void DrawCollider(Rendering.Light.NoSort.Pass pass) {
                int colliderCount = pass.colliderList.Count;

                if (colliderCount < 1) {
                    return;
                }

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
                             Light.Shadow.Shape.Draw(pass.buffer, collider, pass.lightSizeSquared, pass.z);
                        break;
                    }
                }
            }

            public static void DrawTilemapCollider(Rendering.Light.NoSort.Pass pass) {
                #if UNITY_2017_4_OR_NEWER
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingCollisionLayer != pass.layerID) {
                            continue;
                        }

                        switch(pass.tilemapList[id].mapType) {
                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
                                Light.Shadow.TilemapRectangle.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                            break;

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:
                                Light.Shadow.TilemapIsometric.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                        
                            break;

                            case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.Buffer.Shadow_Grid(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                            break;
                        }
                        
                        Light.Shadow.TilemapCollider.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                    }
                #endif 
            }
        }

        public class Mask {

           static public void Draw(Rendering.Light.NoSort.Pass pass) {
                Lighting2D.materials.GetWhiteSprite().SetPass(0);

                GL.Begin(GL.TRIANGLES);

                    GL.Color(Color.white);
                    DrawCollider(pass);

                    GL.Color(Color.white);
                    DrawTilemapCollider(pass);

                GL.End();

                DrawSprite(pass);

                DrawTilemapSprite(pass);
            }

            static public void DrawCollider(Rendering.Light.NoSort.Pass pass) {
                int colliderCount = pass.colliderList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    if ((int)pass.colliderList[id].lightingMaskLayer != pass.layerID) {
                        continue;
                    }

                    switch(pass.colliderList[id].shape.maskType) {
                        case LightingCollider2D.MaskType.Collider:
                            Light.Shape.Mask(pass.buffer, pass.colliderList[id], pass.layer, pass.z);
                        break;

                        case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                            Light.Shape.Mask(pass.buffer, pass.colliderList[id], pass.layer, pass.z);
                        break;

                        case LightingCollider2D.MaskType.Mesh:
                            Mesh.Mask(pass.buffer, pass.colliderList[id], pass.materialWhite, pass.layer, pass.z);
                        break;

                        case LightingCollider2D.MaskType.SkinnedMesh:
                            SkinnedMesh.Mask(pass.buffer, pass.colliderList[id], pass.materialWhite, pass.layer, pass.z);
                        break;
                    }
                }
            }

            static public void DrawSprite(Rendering.Light.NoSort.Pass pass) {
                int colliderCount = pass.colliderList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    LightingCollider2D collider = pass.colliderList[id];

                    if ((int)collider.lightingMaskLayer != pass.layerID) {
                        continue;
                    }

                    switch(collider.shape.maskType) {
                        
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
                    }
                }
            }

            static public void DrawTilemapCollider(Rendering.Light.NoSort.Pass pass) {
                #if UNITY_2017_4_OR_NEWER
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingMaskLayer != pass.layerID) {
                            continue;
                        }

                        switch(pass.tilemapList[id].mapType) {
                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
                                TilemapRectangle.MaskShape(pass.buffer, pass.tilemapList[id], pass.z);
                            break;

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:
                                TilemapIsometric.MaskShape(pass.buffer, pass.tilemapList[id], pass.z);
                            break;

                            case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.Buffer.Mask_Grid(pass.buffer, pass.tilemapList[id], pass.z);
                            break;
                        }
                    }
                #endif
            }

            static public void DrawTilemapSprite(Rendering.Light.NoSort.Pass pass) {
                #if UNITY_2017_4_OR_NEWER
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingMaskLayer != pass.layerID) {
                            continue;
                        }

                        switch(pass.tilemapList[id].mapType) {
                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
                                TilemapRectangle.BumpedSprite(pass.buffer, pass.tilemapList[id], pass.materialNormalMap_PixelToLight, pass.z);
                                TilemapRectangle.Sprite(pass.buffer, pass.tilemapList[id], pass.materialWhite, pass.layer, pass.z);
                            break;

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:
                                TilemapIsometric.MaskSprite(pass.buffer, pass.tilemapList[id], pass.materialWhite, pass.z);
                                
                            break;

                            case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.Buffer.WithoutAtlas.Mask_Sprite(pass.buffer, pass.tilemapList[id], pass.materialWhite, pass.z);
                            break;
                        }                   
                    }
                #endif
            }
        }
    }
}