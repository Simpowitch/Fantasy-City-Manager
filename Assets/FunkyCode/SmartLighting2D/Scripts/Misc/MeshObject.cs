using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshObject {
	public Mesh mesh;
	public Vector3[] vertices;
	public Vector2[] uv;
	public int[] triangles;

	public MeshObject(Mesh meshOrigin) {
		vertices = meshOrigin.vertices;
		uv = meshOrigin.uv;
		triangles = meshOrigin.triangles;

		mesh = meshOrigin;
	}
}