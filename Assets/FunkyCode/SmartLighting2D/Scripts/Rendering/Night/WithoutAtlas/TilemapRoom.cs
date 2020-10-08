using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
	
    public class TilemapRoom {

         static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
            foreach (LightingTilemapRoom2D id in LightingTilemapRoom2D.GetList()) {
                if ((int)id.nightLayer != nightLayer) {
                    continue;
                }

                Material materialColormask = Lighting2D.materials.GetWhiteSprite();
                Material materialMultiply = Lighting2D.materials.GetMultiply();

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
                                
                                Sprite.Draw(camera, id, material, z);

                                material.color = Color.white;
                            break;	

                            case LightingTilemapRoom2D.MapType.SuperTilemapEditor:
                                SuperTilemapEditorSupport.RoomTilemap.DrawTiles(camera, id, material, z);

                            break;
                        }
                        
                    break;
                }			
            }
        }

        public class Collider {

            public static void Draw(Camera camera, LightingTilemapRoom2D id, Material material, float z) {
                material.SetPass (0); 

                if (id.polygonColliders.Count > 0) {
                    foreach(Polygon2D poly in id.polygonColliders) {
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(-camera.transform.position.x, -camera.transform.position.y, z), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1));
                        Vector2 position = -camera.transform.position;
                        Mesh mesh = id.GetPolygonMesh(poly);

                         if (mesh != null) {
                            Graphics.DrawMeshNow(mesh, matrix);
                            GLExtended.DrawMesh(new MeshObject(mesh), -position, Vector2.one, 0);
                        }
                    }
                }
            
                if (id.edgeColliders.Count > 0) {
                    foreach(Polygon2D poly in id.edgeColliders) {
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(-camera.transform.position.x, -camera.transform.position.y, z), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1));
                        Vector2 position = -camera.transform.position;
                        Mesh mesh = id.GetEdgeMesh(poly);

                        if (mesh != null) {
                            Graphics.DrawMeshNow(mesh, matrix);
                            GLExtended.DrawMesh(new MeshObject(mesh), -position, Vector2.one, 0);
                        }
                    }
                }
                
               
            }
        }

        public class Sprite {
            
            public static Vector2D offset = Vector2D.Zero();
            public static Vector2D polyOffset = Vector2D.Zero();
            public static Vector2D tilemapOffset = Vector2D.Zero();
            public static Vector2D inverseOffset = Vector2D.Zero();

            public static Vector2 tileSize = Vector2.zero;
            public static Vector2 scale = Vector2.zero;
            public static Vector2 polyOffset2 = Vector2.zero;

            public static LightingTile tile;

            public static VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

            public static Vector2D tileSize2 = new Vector2D(1, 1);

            public static Vector2Int newPositionInt = new Vector2Int();
            public static Vector2 newPosition = new Vector2();
            public static int sizeInt;
            
            public static void Draw(Camera camera, LightingTilemapRoom2D id, Material material, float z) {
                if (id.map == null) {
                    return;
                }

                SetupLocation(camera, id);

                for(int x = newPositionInt.x - sizeInt; x < newPositionInt.x + sizeInt; x++) {
                    for(int y = newPositionInt.y - sizeInt; y < newPositionInt.y + sizeInt; y++) {
                        if (x < 0 || y < 0) {
                            continue;
                        }

                        if (x >= id.properties.area.size.x || y >= id.properties.area.size.y) {
                            continue;
                        }

                        tile = id.map[x, y];
                        if (tile == null) {
                            continue;
                        }

                        if (tile.GetOriginalSprite() == null) {
                            return;
                        }

                        polyOffset.x = x + tilemapOffset.x;
                        polyOffset.y = y + tilemapOffset.y;

                        polyOffset.x *= scale.x;
                        polyOffset.y *= scale.y;

                        polyOffset2.x = (float)polyOffset.x;
                        polyOffset2.y = (float)polyOffset.y;
                        
                        //if (LightingManager2D.culling && Vector2.Distance(polyOffset2, buffer.lightSource.transform.position) > 2 + buffer.lightSource.lightSize) {
                        //	LightingDebug.culled ++;
                        //	continue;
                        //}

                        polyOffset.x += offset.x;
                        polyOffset.y += offset.y;

                        spriteRenderer.sprite = tile.GetOriginalSprite();

                        polyOffset2.x = (float)polyOffset.x;
                        polyOffset2.y = (float)polyOffset.y;
                    
                        material.mainTexture = spriteRenderer.sprite.texture;
            
                        Rendering.Universal.WithoutAtlas.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, spriteRenderer, polyOffset2, tileSize, 0, z);
                        
                        material.mainTexture = null;
                    }
                }
            }

                    
            static public int LightTilemapSize(LightingTilemapRoom2D id, Camera camera) {
                float ratio = ((float)camera.pixelRect.width) / camera.pixelRect.height;
                return((int)(camera.orthographicSize * ratio * 1.2));
            }

            static public void LightTilemapOffset(LightingTilemapRoom2D id, Vector2 scale, Camera camera) {
                Vector2 position = camera.transform.position;

                position.x -= id.properties.area.position.x;
                position.y -= id.properties.area.position.y;
                
                position.x -= id.transform.position.x;
                position.y -= id.transform.position.y;
                    
                position.x -= id.properties.cellAnchor.x;
                position.y -= id.properties.cellAnchor.y;

                position.x += 1;
                position.y += 1;

                position.x *= scale.x;
                position.y *= scale.y;

                newPositionInt.x = (int)position.x;
                newPositionInt.y = (int)position.y;

                newPosition.x = position.x;
                newPosition.y = position.y;
            }

            static public void SetupLocation(Camera camera, LightingTilemapRoom2D id) {
                Vector3 rot = Math2D.GetPitchYawRollRad(id.transform.rotation);

                float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
                float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);

                scale.x = id.transform.lossyScale.x * rotationXScale * id.properties.cellSize.x;
                scale.y = id.transform.lossyScale.y * rotationYScale * id.properties.cellSize.y;

                sizeInt = LightTilemapSize(id, camera);

                LightTilemapOffset(id, scale, camera);
                
                offset.x = -camera.transform.position.x;
                offset.y = -camera.transform.position.y;

                tilemapOffset.x = id.transform.position.x + id.properties.area.position.x + id.properties.cellAnchor.x;
                tilemapOffset.y = id.transform.position.y + id.properties.area.position.y + id.properties.cellAnchor.y;

                //if (id.mapType == LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
                //	tilemapOffset.x -= id.area.size.x / 2;
                //	tilemapOffset.y -= id.area.size.y / 2;
                //}

                tileSize.x = scale.x / id.properties.cellSize.x;
                tileSize.y = scale.y / id.properties.cellSize.y;
            }
        }
    }
}
