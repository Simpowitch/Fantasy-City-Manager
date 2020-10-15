using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightingShape {
		
	public class Base {
		public List<Polygon2D> polygons_world = null;
		public List<Polygon2D> polygons_world_cache = null;

		public List<Polygon2D> polygons_local = null;

		public List<MeshObject> meshes = null;
		public float meshesRadius = -1f;


		public Transform transform;

		public void SetTransform(Transform t) {
			transform = t;
		}

		virtual public void ResetLocal() {
			meshesRadius = -1f;
			meshes = null;

			polygons_local = null;

			polygons_world = null;
			polygons_world_cache = null;

			ResetWorld();
		}

		virtual public void ResetWorld() {
			polygons_world = null;
		}
	}
}