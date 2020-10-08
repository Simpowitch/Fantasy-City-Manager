using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColliderDepthList {
    public ColliderDepth[] list = new ColliderDepth[1024];

    public int count = 0;

    public ColliderDepthList() {
        for(int i = 0; i < list.Length; i++) {
            list[i] = new ColliderDepth();
        }
    }

    public void Add(LightingCollider2D collider2D, float dist) {
		if (count + 1 < list.Length) {
			list[count].type = ColliderDepth.Type.Collider;
			list[count].collider = collider2D;
			list[count].distance = dist;
			count++;
		} else {
			Debug.LogError("Collider Depth Overhead!");
		}
    }

	#if UNITY_2017_4_OR_NEWER
		public void Add(LightingTilemapCollider2D tilemap, LightingTile tile2D, float dist, Vector2 polyOffset) {
			if (count + 1 < list.Length) {
				list[count].type = ColliderDepth.Type.Tile;
				list[count].tile = tile2D;
				list[count].tilemap = tilemap;
				list[count].distance = dist;
				list[count].polyOffset.x = polyOffset.x;
				list[count].polyOffset.y = polyOffset.y;
				// Tile Size?

				count++;
			} else {
				Debug.LogError("Tile Depth Overhead!");
			}
		}
	#endif

    public void Reset() {
        count = 0;
    }

    public void Sort() {
        Array.Sort<ColliderDepth>(list, 0, count, ColliderDepth.Sort());
    }
}