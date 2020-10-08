using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {
        
    public class Sorted {

         public static void Draw(Rendering.Light.Sorted.Pass pass) {
            Lighting2D.materials.GetAtlasMaterial().SetPass(0);

            GL.Begin(GL.TRIANGLES);
        
            for(int i = 0; i < pass.list.count; i ++) {
                pass.depth = pass.list.list[i];

                switch (pass.depth.type) {
                    case ColliderDepth.Type.Collider:
                        DrawCollder(pass);

                    break;

                    case ColliderDepth.Type.Tile:
                        DrawTilemapCollider(pass);
                    
                    break;
                }
            }
            
            GL.End();
        }

        public static void DrawCollder(Rendering.Light.Sorted.Pass pass) {
            if (pass.drawShadows && (int)pass.depth.collider.lightingCollisionLayer == pass.layerID) {	
                switch(pass.depth.collider.shape.colliderType) {
                    case LightingCollider2D.ColliderType.Collider:
                    case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                        Shadow.Shape.Draw(pass.buffer, pass.depth.collider, pass.lightSizeSquared, pass.z);
                    break;
                }
            }

            if (pass.drawMask && (int)pass.depth.collider.lightingMaskLayer == pass.layerID) {
                switch(pass.depth.collider.shape.maskType) {
                    case LightingCollider2D.MaskType.Collider:
                    case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                        Shape.Mask(pass.buffer, pass.depth.collider, pass.layer, pass.z);  

                    break;

                    case LightingCollider2D.MaskType.Sprite:
                        SpriteRenderer2D.Mask(pass.buffer, pass.depth.collider, pass.z);

                    break;
                }

                // Partialy Batched
                if (pass.buffer.lightingAtlasBatches.colliderList.Count > 0) {
                    DrawColliderBatched(pass);
                }
            }
        }

        public static void DrawTilemapCollider(Rendering.Light.Sorted.Pass pass) {
            if (pass.drawShadows && (int)pass.depth.tilemap.lightingCollisionLayer == pass.layerID) {	
                Shadow.Tile.Draw(pass.buffer, pass.depth.tile, pass.depth.polyOffset, pass.depth.tilemap, pass.lightSizeSquared, pass.z);
            }

            if (pass.drawMask && (int)pass.depth.tilemap.lightingMaskLayer == pass.layerID) {
                Tile.MaskSprite(pass.buffer, pass.depth.tile, pass.layer, pass.depth.tilemap, pass.depth.polyOffset, pass.z);

                // Partialy Batched
                if (pass.buffer.lightingAtlasBatches.tilemapList.Count > 0) {
                    DrawTilemapColliderBatched(pass);
                }
            }   
        }

        public static void DrawColliderBatched(Rendering.Light.Sorted.Pass pass) {
            if (pass.buffer.lightingAtlasBatches.colliderList.Count < 1) {
                return;
            }
            
            GL.End();
                
            for(int s = 0; s < pass.buffer.lightingAtlasBatches.colliderList.Count; s++) {
                pass.batch_collider = pass.buffer.lightingAtlasBatches.colliderList[s];

                if (pass.batch_collider.collider.shape.maskType == LightingCollider2D.MaskType.Sprite) {
                
                    WithoutAtlas.SpriteRenderer2D.Mask(pass.buffer, pass.batch_collider.collider, pass.materialWhite, pass.layer, pass.z);
                }
            }

            pass.buffer.lightingAtlasBatches.colliderList.Clear();

            Lighting2D.materials.GetAtlasMaterial().SetPass(0);
            GL.Begin(GL.TRIANGLES);
        }

        public static void DrawTilemapColliderBatched(Rendering.Light.Sorted.Pass pass) {
            if (pass.buffer.lightingAtlasBatches.tilemapList.Count < 1) {
                return;
            }

            GL.End();

            for(int s = 0; s < pass.buffer.lightingAtlasBatches.tilemapList.Count; s++) {
                pass.batch_tilemap = pass.buffer.lightingAtlasBatches.tilemapList[s];
                
                WithoutAtlas.Tile.MaskSprite(pass.buffer, pass.depth.tile, pass.layer, pass.materialWhite, pass.batch_tilemap.polyOffset, pass.batch_tilemap.tilemap, pass.lightSizeSquared, pass.z);
            }

            pass.buffer.lightingAtlasBatches.tilemapList.Clear();

            Lighting2D.materials.GetAtlasMaterial().SetPass(0);
            GL.Begin(GL.TRIANGLES);
        }
    }
}