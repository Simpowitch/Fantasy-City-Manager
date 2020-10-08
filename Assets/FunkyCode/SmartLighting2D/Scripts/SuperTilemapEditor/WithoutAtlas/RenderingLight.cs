using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTilemapEditorSupport {
    
    public class Buffer : Rendering.Light.Base { 

        public class WithoutAtlas {   

            #if (SUPER_TILEMAP_EDITOR)
            
                static public void Mask_Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material material, float z) {
                    if (id.mapType != LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
                        return;
                    }

                    if (id.superTilemapEditor.maskType != SuperTilemapEditorSupport.TilemapCollider2D.MaskType.Sprite) {
                        return;
                    }

                    if (id.superTilemapEditor.SuperTilemapEditorMap == null) {
                        return;
                    }


                    Vector2 posScale = GetPositionScale(id);
                    Vector2 scale = GetScale(id);
                    Vector2 tilemapOffset = GetTilemapOffsetSTE(id);
                    Vector2 offset = -buffer.lightSource.transform.position;

                    Vector2 polyOffset;
            
                    foreach(TilemapCollider2D.STETile STETile in id.superTilemapEditor.SuperTilemapEditorMap.mapTiles) {
                        LightingTile tile = STETile.tile;
                        int x = STETile.position.x;
                        int y = STETile.position.y; 

                        polyOffset.x = x + tilemapOffset.x;
                        polyOffset.y = y + tilemapOffset.y;

                        polyOffset.x *= posScale.x;
                        polyOffset.y *= posScale.y;

                        if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                            continue;
                        }

                        polyOffset.x += offset.x;
                        polyOffset.y += offset.y;

                        material.mainTexture = id.superTilemapEditor.tilemapSTE.Tileset.AtlasTexture;
                        
                        Rendering.Universal.WithoutAtlas.Texture.Draw(material, polyOffset, scale / 2, STETile.uv, 0, z);
            
                        material.mainTexture = null;
                    }	
                }

            #else 
                static public void Mask_Sprite(LightingBuffer2D buffer, LightingTilemapCollider2D id, Material materialA, float z) {}
            #endif
        }
 
        #if (SUPER_TILEMAP_EDITOR)

            static public void Mask_Grid(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {
                if (id.mapType != LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
                    return;
                }
                
                if (id.superTilemapEditor.maskType != SuperTilemapEditorSupport.TilemapCollider2D.MaskType.Grid) {
                    return;
                }

                if (id.superTilemapEditor.SuperTilemapEditorMap == null) {
                    return;
                }

                Vector2 posScale = GetPositionScale(id);
                Vector2 scale = GetScale(id);
                Vector2 tilemapOffset = GetTilemapOffsetSTE(id);
                Vector2 offset = -buffer.lightSource.transform.position;
                MeshObject tileMesh = LightingTile.GetStaticTileMesh(id);

                Vector2 polyOffset;
    
                if (tileMesh == null) {
                    return;
                }

                GL.Color(Color.white);

                foreach(TilemapCollider2D.STETile STETile in id.superTilemapEditor.SuperTilemapEditorMap.mapTiles) {
                    LightingTile tile = STETile.tile;
                    int x = STETile.position.x;
                    int y = STETile.position.y;
            
                    polyOffset.x = x + tilemapOffset.x;
                    polyOffset.y = y + tilemapOffset.y;

                    polyOffset.x *= posScale.x;
                    polyOffset.y *= posScale.y;

                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    polyOffset.x += offset.x;
                    polyOffset.y += offset.y;

                    GLExtended.DrawMeshPass(tileMesh, polyOffset, scale, 0);		
                }
            }

        #else  
            static public void Mask_Grid(LightingBuffer2D buffer, LightingTilemapCollider2D id, float z) {}

        #endif

        #if (SUPER_TILEMAP_EDITOR)

            static public void Shadow_Grid(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
                if (id.mapType != LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
                    return;
                }

                if (id.superTilemapEditor.colliderType != SuperTilemapEditorSupport.TilemapCollider2D.ColliderType.Grid) {
                    return;
                }

                if (id.superTilemapEditor.SuperTilemapEditorMap == null) {
                    return;
                }

                Vector2 posScale = GetPositionScale(id);
                Vector2 scale = GetScale(id);
                Vector2 tilemapOffset = GetTilemapOffsetSTE(id);
                Vector2 offset = -buffer.lightSource.transform.position;

                Vector2 polyOffset;

                foreach(TilemapCollider2D.STETile STETile in id.superTilemapEditor.SuperTilemapEditorMap.mapTiles) {
                    LightingTile tile = STETile.tile;

                    polygons = tile.GetPolygons(id);

                    if (polygons == null || polygons.Count < 1) {
                        continue;
                    }

                    polyOffset.x = (STETile.position.x + tilemapOffset.x) * posScale.x;
                    polyOffset.y = (STETile.position.y + tilemapOffset.y) * posScale.y;
 
                    if (tile.InRange(polyOffset, buffer.lightSource.transform.position, 2 + buffer.lightSource.size)) {
                        continue;
                    }

                    polyOffset.x += offset.x;
                    polyOffset.y += offset.y;

                    Rendering.Light.Shadow.Polygon.Draw(buffer, polygons, lightSizeSquared, z, polyOffset, scale);
                }
            }

            static public Vector2 GetTilemapOffsetSTE(LightingTilemapCollider2D id) {
                Vector2 tilemapOffset;
                tilemapOffset.x = id.transform.position.x + id.properties.area.position.x + id.properties.cellAnchor.x;
                tilemapOffset.y = id.transform.position.y + id.properties.area.position.y + id.properties.cellAnchor.y;

                tilemapOffset.x -= id.properties.area.size.x / 2;
                tilemapOffset.y -= id.properties.area.size.y / 2;
                return(tilemapOffset);
            }

        #else 
            static public void Shadow_Grid(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {}
        #endif
        
    }
}

   /*
            static public void SetupLocationSTE(LightingBuffer2D buffer, LightingTilemapCollider2D id) {
                LightTilemapOffsetSTE(id, buffer);
            }

            static public void LightTilemapOffsetSTE(LightingTilemapCollider2D id, LightingBuffer2D buffer) {
                Vector2 newPosition = Vector2.zero;
                newPosition.x = buffer.lightSource.transform.position.x;
                newPosition.y = buffer.lightSource.transform.position.y;

                Vector3 rot = Math2D.GetPitchYawRollRad(id.transform.rotation);

                float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);
                float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
            
                float sx = 1; 
                sx /= id.properties.cellSize.x;
                sx /= id.transform.lossyScale.x;
                sx /= rotationXScale;


                float sy = 1;
                sy /= id.properties.cellSize.y;
                sy /= id.transform.lossyScale.y;
                sy /= rotationYScale;


                newPosition.x *= sx;
                newPosition.y *= sy;

                Vector2 tilemapPosition = Vector2.zero;

                tilemapPosition.x -= id.properties.area.position.x;
                tilemapPosition.y -= id.properties.area.position.y;
                
                tilemapPosition.x -= id.transform.position.x;
                tilemapPosition.y -= id.transform.position.y;
                    
                tilemapPosition.x -= id.properties.cellAnchor.x;
                tilemapPosition.y -= id.properties.cellAnchor.y;

                // Cell Size Is Not Calculated Correctly
                tilemapPosition.x += id.properties.area.size.x / 2;
                tilemapPosition.y += id.properties.area.size.y / 2;
              
                newPosition.x += tilemapPosition.x;
                newPosition.y += tilemapPosition.y;

                newPositionInt.x = (int)newPosition.x;
                newPositionInt.y = (int)newPosition.y;
            }*/