using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day.WithoutAtlas {

    public class Sorted {
        static public void Draw(Camera camera, Vector2 offset, float z, Rendering.Day.SortedPass pass) {
            for(int id = 0; id < pass.sortList.count; id++) {
                Sorting.SortObject sortObject = pass.sortList.list[id];

                switch(sortObject.type) {
                    case Sorting.SortObject.Type.Collider:
                        DayLightingCollider2D collider = (DayLightingCollider2D)sortObject.lightObject;

                        if (collider != null) {


                            if (collider.mainShape.colliderType == DayLightingCollider2D.ColliderType.Collider || collider.mainShape.colliderType == DayLightingCollider2D.ColliderType.SpriteCustomPhysicsShape) {
                                Lighting2D.materials.GetShadowBlur().SetPass (0);
                                GL.Begin(GL.TRIANGLES);
                                WithoutAtlas.Shadow.Draw(collider, camera, offset, z);  
                                GL.End(); 
                            }
                            

                            WithoutAtlas.SpriteRendererShadow.Draw(collider, camera, offset, z);
                
                            WithoutAtlas.SpriteRenderer2D.Draw(collider, camera, offset, z);

                        }

                    break;
                }
            }
        }
    }
}
