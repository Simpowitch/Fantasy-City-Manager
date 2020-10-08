using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {

    public class Room {

        static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
            Material material = Lighting2D.materials.GetWhiteSprite();
            Vector2 position;

            foreach (LightingRoom2D id in LightingRoom2D.GetList()) {
                if ((int)id.nightLayer != nightLayer) {
                    continue;
                }

                switch(id.shape.type) {

                    case LightingRoom2D.RoomType.Collider:
                        List<MeshObject> meshObjects = id.shape.GetMeshes();

                        if (meshObjects == null) {
                            continue;
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
            }

            material.color = Color.white;
            material.mainTexture = null;
        }
    }
}