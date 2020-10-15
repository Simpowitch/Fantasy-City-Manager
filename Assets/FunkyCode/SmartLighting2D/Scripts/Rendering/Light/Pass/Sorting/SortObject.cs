using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rendering.Light.Sorting {
	public struct SortObject : System.Collections.Generic.IComparer<SortObject> {
		public enum Type {Collider, Tile};

		public Vector2 position;
		public Type type;
		public float distance;

		public object lightObject;

		// Lighting Tilemap Tile
		#if UNITY_2017_4_OR_NEWER
			public LightingTilemapCollider2D tilemap;
		#endif

		public SortObject(int a) {
			position = Vector2.zero;

			type = Type.Collider;

			distance = 0;

			lightObject = null;

			tilemap = null;
		}

		public int Compare(SortObject a, SortObject b) {
			SortObject c1 = (SortObject)a;
			SortObject c2 = (SortObject)b;

			if (c1.distance > c2.distance) {
				return 1;
			}
		
			if (c1.distance < c2.distance) {
				return -1;
			} else {
				return 0;
			}
		}

		public static System.Collections.Generic.IComparer<SortObject> Sort() {      
			return (System.Collections.Generic.IComparer<SortObject>) new SortObject();
		}
	}
}