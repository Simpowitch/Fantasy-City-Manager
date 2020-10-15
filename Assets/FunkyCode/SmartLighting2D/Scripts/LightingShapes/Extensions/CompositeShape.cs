using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingShape;

namespace LightingShape {

    public class CompositeShape : Base {

        CompositeCollider2D compositeCollider = null;

        public CompositeCollider2D GetCompositeCollider() {
            if (compositeCollider == null) {
                compositeCollider = transform.GetComponent<CompositeCollider2D>();
            }

            return(compositeCollider);
        }

        public List<MeshObject> GetMeshes() {
			if (meshes == null) {
				List<Polygon2D> polygons = GetPolygonsLocal();

				if (polygons.Count > 0) {
					meshes = new List<MeshObject>();
					
					foreach(Polygon2D poly in polygons) {
						if (poly.pointsList.Count < 3) {
							continue;
						}
						
						Mesh mesh = PolygonTriangulator2D.Triangulate (poly, Vector2.zero, Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
						
						if (mesh) {							
							meshes.Add( new MeshObject(mesh) );
						}
					}
				}
			}
			return(meshes);
		}

        public float GetRadius() {
			if (meshesRadius < 0) {
				meshesRadius = 0;

				List<Polygon2D> polygons = GetPolygonsLocal();

				if (polygons.Count > 0) {
					foreach(Polygon2D poly in polygons) {
						foreach (Vector2D id in poly.pointsList) {
							meshesRadius = Mathf.Max(meshesRadius, Vector2.Distance(id.ToVector2(), Vector2.zero));
						}
					}
				}
			}
			return(meshesRadius);
		}

        public List<Polygon2D> GetPolygonsWorld() {
	
			if (polygons_world != null) {
				return(polygons_world);
			}

			if (polygons_world_cache != null) {

				polygons_world = polygons_world_cache;

				Polygon2D poly;
				Vector2D point;
				List<Polygon2D> list = GetPolygonsLocal();

				for(int i = 0; i < list.Count; i++) {
					poly = list[i];
					for(int p = 0; p < poly.pointsList.Count; p++) {
						point = poly.pointsList[p];
						
						polygons_world[i].pointsList[p].x = point.x;
						polygons_world[i].pointsList[p].y = point.y;
					}
					polygons_world[i].ToWorldSpaceItself(transform);
				}

			} else {

				polygons_world = new List<Polygon2D>();
				
				foreach(Polygon2D poly in GetPolygonsLocal()) {
					polygons_world.Add(poly.ToWorldSpace(transform));
				}

				polygons_world_cache = polygons_world;
			
			}
		
			return(polygons_world);
		}

        public List<Polygon2D> GetPolygonsLocal() {
			if (polygons_local != null) {
				return(polygons_local);
			}

            CompositeCollider2D compositeCollider = GetCompositeCollider();

			polygons_local = Polygon2DHelper.CreateFromCompositeCollider(compositeCollider);	

			if (polygons_local.Count <= 0) {
				Debug.LogWarning("SmartLighting2D: LightingCollider2D object is missing CompositeCollider2D Component", transform);
			}
		
			return(polygons_local);
		}
    }
}