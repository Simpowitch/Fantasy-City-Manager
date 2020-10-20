using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
	
    public class TilemapRoom {

        static public void Draw(LightingTilemapRoom2D id, Camera camera, Vector2 offset, float z) {
            Material materialColormask = Lighting2D.materials.GetSpriteMask();
            Material materialMultiply = Lighting2D.materials.GetMultiplyHDR();

            Material material = null;

            switch(id.shaderType) {
                case LightingTilemapRoom2D.ShaderType.ColorMask:
                    material = materialColormask;
                    break;

                case LightingTilemapRoom2D.ShaderType.MultiplyTexture:
                    material = materialMultiply;
                    break;
            }

            switch(id.maskType) {
                case LightingTilemapRoom2D.MaskType.Sprite:
                    
                    switch(id.mapType) {
                        case LightingTilemapRoom2D.MapType.UnityEngineTilemapRectangle:
                            material.color = id.color;
                            
                            Sprite.Draw(camera, id, material, offset, z);

                            material.color = Color.white;
                        break;	

                        case LightingTilemapRoom2D.MapType.SuperTilemapEditor:
                            SuperTilemapEditorSupport.RoomTilemap.DrawTiles(camera, id, material, z);

                        break;
                    }
                    
                break;
            }			
        }

        public class Sprite {
            
            public static VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

            public static void Draw(Camera camera, LightingTilemapRoom2D id, Material material, Vector2 offset, float z) {
                foreach(LightingTile tile in id.rectangle.mapTiles) {
                    if (tile.GetOriginalSprite() == null) {
                       continue;
                    }

                    Vector2 tilePosition = Rendering.Light.Rectangle.GetTilePosition(tile.position.x, tile.position.y, id.transform, id.GetTilemapProperties());

                    tilePosition += offset;

                    if (tile.InRange(tilePosition, camera.orthographicSize * 2)) {
                       continue;
                    }

                    spriteRenderer.sprite = tile.GetOriginalSprite();
                
                    material.mainTexture = spriteRenderer.sprite.texture;
        
                    Rendering.Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, spriteRenderer, tilePosition, Vector2.one, 0, z);
                    
                    material.mainTexture = null;
                }
            }
        }
    }
}
