using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEvents {
	private static bool initialized = false;

	public static void Initialize() {
		if (initialized) {
			return;
		}

		initialized = true;

		#if UNITY_EDITOR

			#if UNITY_2019_3_OR_NEWER
				Tilemap.tilemapTileChanged -= Events;
				Tilemap.tilemapTileChanged += Events;
			#endif

		#endif
	}
	
	#if UNITY_EDITOR
		#if UNITY_2019_3_OR_NEWER
			public static void Events(Tilemap tilemap, Tilemap.SyncTile[] s) {
				if (Application.isPlaying) {
					return;
				}

				foreach(LightingTilemapCollider2D tilemap2D in LightingTilemapCollider2D.GetList()) {
					tilemap2D.Initialize();
				}
				
				LightingSource2D.ForceUpdateAll();
			}
		#endif
	#endif
}