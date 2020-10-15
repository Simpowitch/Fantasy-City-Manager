using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTilemapEditorSupport {

    public class RoomTilemap {
        #if (SUPER_TILEMAP_EDITOR)

            public static void DrawTiles(Camera camera, LightingTilemapRoom2D id, Material material, float z) {
                Vector2 posScale = Vector2.one;

                MeshObject mesh = id.superTilemapEditor.GetSTEMesh();

                if (mesh == null) {
                    return;
                }

                float of_x = Mathf.Abs(id.properties.area.position.x - id.superTilemapEditor.tilemapSTE.MapBounds.extents.x / 2) * 2;
                float of_y = Mathf.Abs(id.properties.area.position.y - id.superTilemapEditor.tilemapSTE.MapBounds.extents.y / 2) * 2;

                float x = -camera.transform.position.x;
                float y = -camera.transform.position.y;

                x -= (of_x ) / 2 + 0.5f;
                y -= (of_y ) / 2  + 0.5f;

                x -= id.superTilemapEditor.tilemapSTE.MapBounds.extents.x / 2;
                y -= id.superTilemapEditor.tilemapSTE.MapBounds.extents.y / 2;

                material.mainTexture = id.superTilemapEditor.tilemapSTE.Tileset.AtlasTexture;

                material.color = id.color;

                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1));

                material.SetPass (0);
                Graphics.DrawMeshNow(mesh.mesh, matrix);
             
                // GLExtended.DrawMesh(mesh, new Vector3(x, y, 0), new Vector2(1, 1), 0);

                material.mainTexture = null;
            }
        #else 
            public static void DrawTiles(Camera camera, LightingTilemapRoom2D id, Material material, float z) {}
        #endif

    }

}