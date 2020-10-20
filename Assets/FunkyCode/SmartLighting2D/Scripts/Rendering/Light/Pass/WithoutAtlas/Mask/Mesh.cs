using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class Mesh {
        
        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

			foreach(LightingColliderShape shape in id.shapes) {
				MeshRenderer meshRenderer = id.mainShape.meshShape.GetMeshRenderer();
				
				if (meshRenderer == null) {
					return;
				}

				List<MeshObject> meshObjects = id.mainShape.GetMeshes();

				if (meshObjects == null) {
					return;
				}

				if (meshRenderer.sharedMaterial != null) {
					material.mainTexture = meshRenderer.sharedMaterial.mainTexture;
				} else {
					material.mainTexture = null;
				}

				Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;
				GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

				material.SetPass(0);

				GLExtended.DrawMesh(meshObjects, position, Vector2.one, shape.transform2D.rotation);
				
				material.mainTexture = null;
			}
		}
    }
}