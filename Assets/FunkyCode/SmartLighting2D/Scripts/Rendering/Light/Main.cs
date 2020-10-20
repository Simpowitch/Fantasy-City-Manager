using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
	
	public class Main {

		public static void Draw(LightingBuffer2D buffer) {
			if (buffer.lightSource == null) {
				return;
			}

			ShadowEngine.Prepare(buffer.lightSource);

            LayerSetting[] layerSettings = buffer.lightSource.GetLayerSettings();

			if (layerSettings != null) {

				if (layerSettings.Length > 0) {
					
					for (int layerID = 0; layerID < layerSettings.Length; layerID++) {
						LayerSetting layerSetting = layerSettings[layerID];

						if (layerSetting == null) {
							continue;
						}

						if (layerSetting.sorting == LightingLayerSorting.None) {
							Light.NoSort.Draw(buffer, layerSetting);
						} else {
							Light.Sorted.Draw(buffer, layerSetting);
						}
					}
				}

			}
			
			Light.LightSource.Main.Draw(buffer);
		}
	}
}