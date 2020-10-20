using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {
        
    public class SkinnedMesh {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

			foreach(LightingColliderShape shape in id.shapes) {
				SkinnedMeshRenderer skinnedMeshRenderer = shape.skinnedMeshShape.GetSkinnedMeshRenderer();

				if (skinnedMeshRenderer == null) {
					return;
				}

				List<MeshObject> meshObject = shape.GetMeshes();

				if (meshObject == null) {
					return;
				}

				if (skinnedMeshRenderer.sharedMaterial != null) {
					material.mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
				} else {
					material.mainTexture = null;
				}

				Vector2 position = shape.transform2D.position - buffer.lightSource.transform2D.position;
				GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

				material.SetPass(0);

				GLExtended.DrawMesh(meshObject, position, Vector2.one, shape.transform2D.rotation);

				material.mainTexture = null;
			}
		}
    }
}