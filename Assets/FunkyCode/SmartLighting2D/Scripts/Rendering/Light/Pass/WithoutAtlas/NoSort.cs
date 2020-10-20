using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class NoSort {

        public static void Draw(Rendering.Light.Pass pass) {
            if (pass.drawShadows) {
                Shadow.Draw(pass);
            }

            if (pass.drawMask) {
                Mask.Draw(pass);
            }
        }

        public class Shadow {

            public static void Draw(Rendering.Light.Pass pass) {
                Material material = Lighting2D.materials.GetAtlasMaterial();
                material.color = Color.white;

                material.SetPass(0);

                GL.Begin(GL.TRIANGLES);

                DrawCollider(pass);
                DrawTilemapCollider(pass);
        
                GL.End();
            }

            public static void DrawCollider(Rendering.Light.Pass pass) {
                int colliderCount = pass.layerCollisionList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    LightingCollider2D collider = pass.layerCollisionList[id];

                    switch(collider.mainShape.colliderType) {
                        case LightingCollider2D.ColliderType.Collider2D:
                        case LightingCollider2D.ColliderType.CompositeCollider2D:
                        case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                        case LightingCollider2D.ColliderType.MeshRenderer:
                        case LightingCollider2D.ColliderType.SkinnedMeshRenderer:
                             Light.Shadow.Shape.Draw(pass.buffer, collider);
                        break;
                    }
                }
            }

            public static void DrawTilemapCollider(Rendering.Light.Pass pass) {
                #if UNITY_2017_4_OR_NEWER
                    for(int id = 0; id < pass.tilemapList.Count; id++) {
                        if ((int)pass.tilemapList[id].lightingCollisionLayer != pass.layerID) {
                            continue;
                        }

                        switch(pass.tilemapList[id].mapType) {
                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
                                Light.Shadow.TilemapRectangle.Draw(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                                Light.Shadow.TilemapCollider.Rectangle.Draw(pass.buffer, pass.tilemapList[id]);
                            break;

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:
                                Light.Shadow.TilemapIsometric.Draw(pass.buffer, pass.tilemapList[id]);
                            break;

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon:
                                Light.Shadow.TilemapHexagon.Draw(pass.buffer, pass.tilemapList[id]);
                            break;

                            case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.Buffer.Shadow_Grid(pass.buffer, pass.tilemapList[id], pass.lightSizeSquared, pass.z);
                            break;
                        }
                        
                        
                    }
                #endif 
            }
        }

        public class Mask {

           static public void Draw(Rendering.Light.Pass pass) {
                Lighting2D.materials.GetSpriteMask().SetPass(0);

                GL.Begin(GL.TRIANGLES);

                    GL.Color(Color.white);
                    DrawCollider(pass);

                    GL.Color(Color.white);
                    DrawTilemapCollider(pass);

                GL.End();

                DrawMesh(pass);

                DrawSprite(pass);

                DrawTilemapSprite(pass);
            }

            static public void DrawCollider(Rendering.Light.Pass pass) {
                int colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    LightingCollider2D collider = pass.layerMaskList[id];

                    switch(collider.mainShape.maskType) {
                        case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                            Light.Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                        break;

                        case LightingCollider2D.MaskType.Collider2D:
                            Light.Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                        break;
             
                        case LightingCollider2D.MaskType.CompositeCollider2D:
                            Light.Shape.Mask(pass.buffer, collider, pass.layer, pass.z);
                        break;
                    }
                }
            }

             static public void DrawMesh(Rendering.Light.Pass pass) {
                int colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    LightingCollider2D collider = pass.layerMaskList[id];

                    switch(collider.mainShape.maskType) {
                        case LightingCollider2D.MaskType.MeshRenderer:
                            Mesh.Mask(pass.buffer, collider, pass.materialWhite, pass.layer, pass.z);
                        break;

                        case LightingCollider2D.MaskType.SkinnedMeshRenderer:
                            SkinnedMesh.Mask(pass.buffer, collider, pass.materialWhite, pass.layer, pass.z);
                        break;
                    }
                }
            }

            static public void DrawSprite(Rendering.Light.Pass pass) {
                int colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) {
                    return;
                }

                for(int id = 0; id < colliderCount; id++) {
                    LightingCollider2D collider = pass.layerMaskList[id];

                    switch(collider.mainShape.maskType) {
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

            static public void DrawTilemapCollider(Rendering.Light.Pass pass) {
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

                            case LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon:
                                TilemapHexagon.MaskShape(pass.buffer, pass.tilemapList[id], pass.z);
                            break;

                            case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.Buffer.MaskGrid(pass.buffer, pass.tilemapList[id], pass.z);
                            break;
                        }
                    }
                #endif
            }

            static public void DrawTilemapSprite(Rendering.Light.Pass pass) {
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