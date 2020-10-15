using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class TilemapRectangle : Base {
        
        static public void Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.rectangle.maskType != LightingTilemapCollider.Rectangle.MaskType.Sprite) {
                return;
            }

            if (id.rectangle == null) {
                return;
            }
            
            if (id.rectangle.map == null) {
                return;
            }

            TilemapProperties properties = id.rectangle.Properties;
            Vector2 offset = -buffer.lightSource.transform.position;
            bool isGrid = false;

            Vector2 scale = Rectangle.GetScale(id, isGrid);
           
            int tilemapSize = Rectangle.Light.GetSize(id, buffer);
			Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= properties.arraySize.x || y >= properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangle.map.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    if (tile.GetOriginalSprite() == null) {
                        return;
                    }

                    Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

                    tilePosition += offset;

                    if (tile.InRange(tilePosition, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                    material.color = LayerSettingColor.Get(tilePosition, layerSetting, MaskEffect.Lit);
                    
                    material.mainTexture = virtualSpriteRenderer.sprite.texture;
        
                    Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale, 0, z);
                    
                    material.mainTexture = null;
                }	
            }
        }

        static public void BumpedSprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.rectangle.maskType != LightingTilemapCollider.Rectangle.MaskType.BumpedSprite) {
                return;
            }

            if (id.rectangle == null) {
                return;
            }
            
            if (id.rectangle.map == null) {
                return;
            }
            
            Texture bumpTexture = id.bumpMapMode.GetBumpTexture();

            if (bumpTexture == null) {
                return;
            }

            material.SetTexture("_Bump", bumpTexture);

            TilemapProperties properties = id.rectangle.Properties;
            Vector2 offset = -buffer.lightSource.transform.position;
            bool isGrid = false;

            Vector2 scale = Rectangle.GetScale(id, isGrid);

            int tilemapSize = Rectangle.Light.GetSize(id, buffer);
			Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);
           
            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= properties.arraySize.x || y >= properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangle.map.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    if (tile.GetOriginalSprite() == null) {
                        return;
                    }

                    Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

                    tilePosition += offset;

                    if (tile.InRange(tilePosition, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                    material.mainTexture = virtualSpriteRenderer.sprite.texture;
        
                    Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale, 0, z);

                    material.mainTexture = null;
                }	
            }
        }

        static public void MaskShape(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }
            
            if (false == (id.rectangle.maskType == LightingTilemapCollider.Rectangle.MaskType.SpriteCustomPhysicsShape || id.rectangle.maskType == LightingTilemapCollider.Rectangle.MaskType.Grid)) {
                return;
            }

            if (id.rectangle == null) {
                return;
            }

            if (id.rectangle.map == null) {
                return;
            }

            TilemapProperties properties = id.rectangle.Properties;
            Vector2 offset = -buffer.lightSource.transform.position;
            bool isGrid = id.rectangle.maskType == LightingTilemapCollider.Rectangle.MaskType.Grid;

            Vector2 scale = Rectangle.GetScale(id, isGrid);

            int tilemapSize = Rectangle.Light.GetSize(id, buffer);
			Vector2Int tilemapLightPosition = Rectangle.Light.GetPosition(id, buffer);

            MeshObject tileMesh = null;	
            if (isGrid) {
                tileMesh = LightingTile.GetStaticTileMesh(id);
            }

            GL.Color(Color.white);

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= properties.arraySize.x || y >= properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangle.map.map[x, y];
                    if (tile == null) {
                        continue;
                    }
                    
                    Vector2 tilePosition = Rectangle.GetTilePosition(x, y, id);

                    tilePosition += offset;
                    
                    if (tile.InRange(tilePosition, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    if (isGrid == false) {
                        tileMesh = null;
                        tileMesh = tile.GetTileDynamicMesh();
                    }

                    if (tileMesh == null) {
                        continue;
                    }

                    GLExtended.DrawMeshPass(tileMesh, tilePosition, scale, 0);		
                }
            }
        }
    }
}

