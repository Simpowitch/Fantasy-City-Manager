using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.WithoutAtlas {

    public class Mesh : Base {
        
        public static void Mask(LightingBuffer2D buffer, LightingCollider2D id, Material material, LayerSetting layerSetting, float z) {
			if (id.InLightSource(buffer) == false) {
				return;
			}

			MeshRenderer meshRenderer = id.shape.meshShape.GetMeshRenderer();
			
			if (meshRenderer == null) {
				return;
			}

			List<MeshObject> meshObjects = id.shape.GetMeshes();

			if (meshObjects == null) {
				return;
			}

			if (meshRenderer.sharedMaterial != null) {
				material.mainTexture = meshRenderer.sharedMaterial.mainTexture;
			} else {
				material.mainTexture = null;
			}

			Vector2 position = id.transform.position - buffer.lightSource.transform.position;

			GL.Color(LayerSettingColor.Get(position, layerSetting, id.maskEffect));

			GLExtended.DrawMeshPass(meshObjects, position, Vector2.one, id.transform2D.rotation);

			material.mainTexture = null;
		}
    }
}