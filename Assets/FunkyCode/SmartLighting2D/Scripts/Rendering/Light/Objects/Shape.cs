using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
    public class Shape : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, LayerSetting layerSetting, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

            List<MeshObject> meshObjects = id.shape.GetMeshes();

			if (meshObjects == null) {
				return;
			}

            Vector2 position = id.transform.position - buffer.lightSource.transform.position;
            
            GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

            GLExtended.DrawMeshPass(meshObjects, position, Vector2.one, id.transform2D.rotation);
        }
    }
}