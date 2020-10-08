using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct ColliderDepth : System.Collections.Generic.IComparer<ColliderDepth> {
	public enum Type {Collider, Tile};

	public Vector2 polyOffset;
	public Type type;
	public float distance;

	// Lighting Collider
	public LightingCollider2D collider;

	// Lighting Tilemap Tile
	#if UNITY_2017_4_OR_NEWER
		public LightingTile tile;

		public LightingTilemapCollider2D tilemap;
	#endif

	public ColliderDepth(int a) {
		polyOffset = Vector2.zero;

		type = Type.Collider;

		distance = 0;

		collider = null;

		tile = null;
		tilemap = null;
	}

	public int Compare(ColliderDepth a, ColliderDepth b) {
		ColliderDepth c1 = (ColliderDepth)a;
		ColliderDepth c2 = (ColliderDepth)b;

		if (c1.distance > c2.distance) {
			return 1;
		}
	
		if (c1.distance < c2.distance) {
			return -1;
		} else {
			return 0;
		}
	}

	public static System.Collections.Generic.IComparer<ColliderDepth> Sort() {      
		return (System.Collections.Generic.IComparer<ColliderDepth>) new ColliderDepth();
	}
}