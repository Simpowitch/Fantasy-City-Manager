using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2 {
	public Vector2[] points;

	public Polygon2(Polygon2D polygon) {
		points = new Vector2[polygon.pointsList.Count];

		for(int id = 0; id < polygon.pointsList.Count; id++) {
			points[id] = polygon.pointsList[id].ToVector2();
		}
	}

	public void ToWorldSpaceSelf(Transform transform) {
		for(int id = 0; id < points.Length; id++) {
			points[id] = transform.TransformPoint (points[id]);
		}
	}
	
	public void ToOffsetItself(Vector2 pos) {
		for(int id = 0; id < points.Length; id++) {
			points[id] += pos;
		}
	}
}