using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class TilemapRectangle : Base {
        
        static public void Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, LayerSetting layerSetting, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.maskType != LightingTilemapCollider2D.MaskType.Sprite) {
                return;
            }

            if (id.rectangleMap == null) {
                return;
            }
            
            if (id.rectangleMap.map == null) {
                return;
            }

            Vector2 positionScale = GetPositionScale(id);
            Vector2 tilemapOffset = GetTilemapOffset(id);
            int tilemapSize = GetTilemapSize(id, buffer);
            Vector2 offset = -buffer.lightSource.transform.position;
            Vector2Int tilemapLightPosition = GetTilemapLightPosition(id, buffer);

            Vector2 polyOffset;
            
            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= id.properties.arraySize.x || y >= id.properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangleMap.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    if (tile.GetOriginalSprite() == null) {
                        return;
                    }

                    polyOffset.x = x + tilemapOffset.x;
                    polyOffset.y = y + tilemapOffset.y;

                    polyOffset.x *= positionScale.x;
                    polyOffset.y *= positionScale.y;

                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    polyOffset += offset;

                    virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                    material.color = LayerSettingColor.Get(polyOffset, layerSetting, MaskEffect.Lit);
                    
                    material.mainTexture = virtualSpriteRenderer.sprite.texture;
        
                    Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, polyOffset, id.transform.lossyScale, 0, z);
                    
                    material.mainTexture = null;
                }	
            }
        }

        static public void BumpedSprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }

            if (id.maskType != LightingTilemapCollider2D.MaskType.BumpedSprite) {
                return;
            }

            if (id.rectangleMap == null) {
                return;
            }
            
            if (id.rectangleMap.map == null) {
                return;
            }
            
            Texture bumpTexture = id.bumpMapMode.GetBumpTexture();

            if (bumpTexture == null) {
                return;
            }

            material.SetTexture("_Bump", bumpTexture);


            Vector2 positionScale = GetPositionScale(id);
            Vector2 tilemapOffset = GetTilemapOffset(id);
            int tilemapSize = GetTilemapSize(id, buffer);
            Vector2 offset = -buffer.lightSource.transform.position;
            Vector2Int tilemapLightPosition = GetTilemapLightPosition(id, buffer);

            Vector2 polyOffset;

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= id.properties.arraySize.x || y >= id.properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangleMap.map[x, y];
                    if (tile == null) {
                        continue;
                    }

                    if (tile.GetOriginalSprite() == null) {
                        return;
                    }

                    polyOffset.x = x + tilemapOffset.x;
                    polyOffset.y = y + tilemapOffset.y;

                    polyOffset.x *= positionScale.x;
                    polyOffset.y *= positionScale.y;

                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    polyOffset += offset;

                    virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                    material.mainTexture = virtualSpriteRenderer.sprite.texture;
        
                    Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, polyOffset, id.transform.lossyScale, 0, z);

                    material.mainTexture = null;
                }	
            }
        }

        static public void MaskShape(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle) {
                return;
            }
            
            if (false == (id.maskType == LightingTilemapCollider2D.MaskType.SpriteCustomPhysicsShape || id.maskType == LightingTilemapCollider2D.MaskType.Grid)) {
                return;
            }

            if (id.rectangleMap == null) {
                return;
            }

            if (id.rectangleMap.map == null) {
                return;
            }

            MeshObject tileMesh = null;	

            Vector2 positionScale = GetPositionScale(id);
            Vector2 scale = GetScale(id);
            int tilemapSize = GetTilemapSize(id, buffer);
            Vector2 tilemapOffset = GetTilemapOffset(id);
            Vector2 offset = -buffer.lightSource.transform.position;
            Vector2Int tilemapLightPosition = GetTilemapLightPosition(id, buffer);

            Vector2 polyOffset;
          
            if (id.maskType == LightingTilemapCollider2D.MaskType.Grid) {
                tileMesh = LightingTile.GetStaticTileMesh(id);
            }

            GL.Color(Color.white);

            for(int x = tilemapLightPosition.x - tilemapSize; x < tilemapLightPosition.x + tilemapSize; x++) {
                for(int y = tilemapLightPosition.y - tilemapSize; y < tilemapLightPosition.y + tilemapSize; y++) {
                    if (x < 0 || y < 0 || x >= id.properties.arraySize.x || y >= id.properties.arraySize.y) {
                        continue;
                    }

                    LightingTile tile = id.rectangleMap.map[x, y];
                    if (tile == null) {
                        continue;
                    }
                    
                    polyOffset.x = x + tilemapOffset.x;
                    polyOffset.y = y + tilemapOffset.y;

                    polyOffset.x *= positionScale.x;
                    polyOffset.y *= positionScale.y;

                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    polyOffset += offset;

                    if (id.maskType == LightingTilemapCollider2D.MaskType.SpriteCustomPhysicsShape) {
                        tileMesh = null;
                        tileMesh = tile.GetTileDynamicMesh();
                    }

                    if (tileMesh == null) {
                        continue;
                    }

                    GLExtended.DrawMeshPass(tileMesh, polyOffset, id.transform.lossyScale, 0);		
                }
            }
        }
    }
}

