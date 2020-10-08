using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {
        
    public class SkinnedMesh : Base {

        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

			SkinnedMeshRenderer skinnedMeshRenderer = id.shape.skinnedMeshShape.GetSkinnedMeshRenderer();

			if (skinnedMeshRenderer == null) {
				return;
			}

			List<MeshObject> meshObject = id.shape.GetMeshes();

			if (meshObject == null) {
				return;
			}

			if (skinnedMeshRenderer.sharedMaterial != null) {
				material.mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
			} else {
				material.mainTexture = null;
			}

			Vector2 position = id.transform.position - buffer.lightSource.transform.position;

			GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

			GLExtended.DrawMeshPass(meshObject, position, Vector2.one, id.transform2D.rotation);

			material.mainTexture = null;
		}
    }
}