using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
    public class Shape : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

            foreach(LightingColliderShape shape in id.shapes) {
                List<MeshObject> meshObjects = shape.GetMeshes();

                if (meshObjects == null) {
                    return;
                }
                            
                Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;
                GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

                GLExtended.DrawMeshPass(meshObjects, position, Vector2.one, shape.transform2D.rotation);
            }
        }
    }
}