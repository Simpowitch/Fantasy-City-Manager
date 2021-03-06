﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightingShape {
		
	public class MeshShape : Base {
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		public MeshFilter GetMeshFilter() {
			if (meshFilter == null) {
				if (transform != null) {
					meshFilter = transform.GetComponent<MeshFilter>();
				}
			}
			return(meshFilter);
		}

		public MeshRenderer GetMeshRenderer() {
			if (meshRenderer == null) {
				if (transform != null) {
					meshRenderer = transform.GetComponent<MeshRenderer>();
				}
			}
			return(meshRenderer);
		}

		public List<MeshObject> GetMeshes() {
			if (meshes == null) {
				if (GetMeshFilter() != null) {
					Mesh mesh = GetMeshFilter().sharedMesh;

					if (mesh) {
						meshes = new List<MeshObject>();
						meshes.Add( new MeshObject(mesh) );
					}
				}
			}
			return(meshes);
		}

		public List<Polygon2D> GetPolygonsWorld() {
			if (polygons_world != null) {
				return(polygons_world);
			}

			List<MeshObject> meshes = GetMeshes();

			if (meshes == null) {
				polygons_world = new List<Polygon2D>();
				return(polygons_world);
			}

			MeshObject meshObject = meshes[0];

			if (meshObject == null) {
				polygons_world = new List<Polygon2D>();
				return(polygons_world);
			}

			Vector3 vecA, vecB, vecC;
			Polygon2D poly;

			if (polygons_world_cache == null) {
				polygons_world = new List<Polygon2D>();

				for (int i = 0; i < meshObject.triangles.GetLength (0); i = i + 3) {
					vecA = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i]]);
					vecB = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 1]]);
					vecC = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 2]]);

					poly = new Polygon2D();
					poly.AddPoint(vecA.x, vecA.y);
					poly.AddPoint(vecB.x, vecB.y);
					poly.AddPoint(vecC.x, vecC.y);

					polygons_world.Add(poly);
				}	
				polygons_world_cache = polygons_world;

			} else {
				int count = 0;

				polygons_world = polygons_world_cache;

				for (int i = 0; i < meshObject.triangles.GetLength (0); i = i + 3) {
					vecA = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i]]);
					vecB = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 1]]);
					vecC = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 2]]);

					poly = polygons_world[count];
					poly.pointsList[0].x = vecA.x;
					poly.pointsList[0].y = vecA.y;
					poly.pointsList[1].x = vecB.x;
					poly.pointsList[1].y = vecB.y;
					poly.pointsList[2].x = vecC.x;
					poly.pointsList[2].y = vecC.y;

					count += 1;
				}
			}
		
			return(polygons_world);
		}
	}
}