using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {

    public class Room {

        public static void Draw(LightingRoom2D id, Camera camera, Vector2 offset, float z) {
            Material material = Lighting2D.materials.GetSpriteMask();
 
            Vector2 position;

            switch(id.shape.type) {

                case LightingRoom2D.RoomType.Collider:
                    List<MeshObject> meshObjects = id.shape.GetMeshes();

                    if (meshObjects == null) {
                        return;
                    }

                    //Vector3 matrixPosition = new Vector3(id.transform.position.x, id.transform.position.y, z);
                    //Quaternion matrixRotation = Quaternion.Euler(0, 0, id.transform.rotation.eulerAngles.z);
                    //Vector3 matrixScale = new Vector3(id.transform.lossyScale.x, id.transform.lossyScale.y, 1);
                    //Graphics.DrawMeshNow(meshObject.mesh, Matrix4x4.TRS(matrixPosition, matrixRotation, matrixScale));

                    material.color = id.color;
                    material.mainTexture = null;
                    material.SetPass(0);

                    position = id.transform.position;
                    position += offset;

                    GLExtended.DrawMesh(meshObjects, position, id.transform.lossyScale, id.transform.rotation.eulerAngles.z);
                
                break;

                case LightingRoom2D.RoomType.Sprite:
                    UnityEngine.SpriteRenderer spriteRenderer = id.shape.spriteShape.GetSpriteRenderer();
                    
                    if (spriteRenderer == null) {
                        return;
                    }

                    Sprite sprite = spriteRenderer.sprite;;
                    if (sprite == null ) {
                        return;
                    }

                    
                    material.mainTexture = sprite.texture;
                    material.color = id.color;

                    position = id.transform.position;
                    position += offset;

                    Rendering.Universal.WithoutAtlas.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, id.transform.lossyScale, id.transform.eulerAngles.z, z);	

                break;
            }

            material.color = Color.white;
            material.mainTexture = null;
        }
    }
}