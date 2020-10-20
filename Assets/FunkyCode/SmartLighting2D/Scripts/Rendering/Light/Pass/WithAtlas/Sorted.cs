using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithAtlas {
        
    public class Sorted {

         public static void Draw(Rendering.Light.Pass pass) {
            Lighting2D.materials.GetAtlasMaterial().SetPass(0);

            GL.Begin(GL.TRIANGLES);
        
            for(int i = 0; i < pass.sortPass.sortList.count; i ++) {
                pass.sortPass.sortObject = pass.sortPass.sortList.list[i];

                switch (pass.sortPass.sortObject.type) {
                    case Sorting.SortObject.Type.Collider:
                        DrawCollder(pass);

                    break;

                    case Sorting.SortObject.Type.Tile:
                        DrawTilemapCollider(pass);
                    
                    break;
                }
            }
            
            GL.End();
        }

        public static void DrawCollder(Rendering.Light.Pass pass) {
            LightingCollider2D collider = (LightingCollider2D)pass.sortPass.sortObject.lightObject;

            if (pass.drawShadows && (int)collider.lightingCollisionLayer == pass.layerID) {	
                switch(collider.mainShape.colliderType) {
                    case LightingCollider2D.ColliderType.Collider2D:
                    case LightingCollider2D.ColliderType.SpriteCustomPhysicsShape:
                        Shadow.Shape.Draw(pass.buffer, collider);
                    break;
                }
            }

            if (pass.drawMask && (int)collider.lightingMaskLayer == pass.layerID) {
                switch(collider.mainShape.maskType) {
                    case LightingCollider2D.MaskType.Collider2D:
                    case LightingCollider2D.MaskType.SpriteCustomPhysicsShape:
                     //   Shape.Mask(pass.buffer, collider, pass.layer, pass.z);  

                    break;

                    case LightingCollider2D.MaskType.Sprite:
                        SpriteRenderer2D.Mask(pass.buffer, collider, pass.z);

                    break;
                }

                // Partialy Batched
                if (pass.buffer.lightingAtlasBatches.colliderList.Count > 0) {
                    DrawColliderBatched(pass);
                }
            }
        }

        public static void DrawTilemapCollider(Rendering.Light.Pass pass) {
            LightingTile tile = (LightingTile)pass.sortPass.sortObject.lightObject;

            if (pass.drawShadows && (int)pass.sortPass.sortObject.tilemap.lightingCollisionLayer == pass.layerID) {	
                Shadow.Tile.Draw(pass.buffer, tile, pass.sortPass.sortObject.position, pass.sortPass.sortObject.tilemap);
            }

            if (pass.drawMask && (int)pass.sortPass.sortObject.tilemap.lightingMaskLayer == pass.layerID) {
                Tile.MaskSprite(pass.buffer, tile, pass.layer, pass.sortPass.sortObject.tilemap, pass.sortPass.sortObject.position, pass.z);

                // Partialy Batched
                if (pass.buffer.lightingAtlasBatches.tilemapList.Count > 0) {
                    DrawTilemapColliderBatched(pass);
                }
            }   
        }

        public static void DrawColliderBatched(Rendering.Light.Pass pass) {
            if (pass.buffer.lightingAtlasBatches.colliderList.Count < 1) {
                return;
            }
            
            GL.End();

                
            for(int s = 0; s < pass.buffer.lightingAtlasBatches.colliderList.Count; s++) {
                PartiallyBatchedCollider batch_collider = pass.buffer.lightingAtlasBatches.colliderList[s];

                if (batch_collider.collider.mainShape.maskType == LightingCollider2D.MaskType.Sprite) {
                
                    WithoutAtlas.SpriteRenderer2D.Mask(pass.buffer, batch_collider.collider, pass.materialWhite, pass.layer, pass.z);
                }
            }

            pass.buffer.lightingAtlasBatches.colliderList.Clear();

            Lighting2D.materials.GetAtlasMaterial().SetPass(0);
            GL.Begin(GL.TRIANGLES);
        }

        public static void DrawTilemapColliderBatched(Rendering.Light.Pass pass) {
            if (pass.buffer.lightingAtlasBatches.tilemapList.Count < 1) {
                return;
            }

            GL.End();

            for(int s = 0; s < pass.buffer.lightingAtlasBatches.tilemapList.Count; s++) {
                PartiallyBatchedTilemap batch_tilemap = pass.buffer.lightingAtlasBatches.tilemapList[s];

                LightingTile tile = (LightingTile)pass.sortPass.sortObject.lightObject;
                
                WithoutAtlas.Tile.MaskSprite(pass.buffer, tile, pass.layer, pass.materialWhite, batch_tilemap.polyOffset, batch_tilemap.tilemap, pass.lightSizeSquared, pass.z);
            }

            pass.buffer.lightingAtlasBatches.tilemapList.Clear();

            Lighting2D.materials.GetAtlasMaterial().SetPass(0);
            GL.Begin(GL.TRIANGLES);
        }
    }
}