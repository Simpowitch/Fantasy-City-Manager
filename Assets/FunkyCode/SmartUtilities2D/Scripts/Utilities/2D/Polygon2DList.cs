using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2DHelper  {
	public enum ColliderType {Polygon, Box, Circle, Capsule, Edge, None}
	public enum PolygonType {Rectangle, Circle, Pentagon, Hexagon, Octagon};
	static public int defaultCircleVerticesCount = 25;

	public static ColliderType GetColliderType(GameObject gameObject) {
		EdgeCollider2D edgeCollider2D = gameObject.GetComponent<EdgeCollider2D> ();
		if (edgeCollider2D != null) {
			return(ColliderType.Edge);
		}

		PolygonCollider2D polygonCollider2D = gameObject.GetComponent<PolygonCollider2D> ();
		if (polygonCollider2D != null) {
			return(ColliderType.Polygon);
		}

		BoxCollider2D boxCollider2D = gameObject.GetComponent<BoxCollider2D> ();
		if (boxCollider2D != null) {
			return(ColliderType.Box);
		}

		CircleCollider2D circleCollider2D = gameObject.GetComponent<CircleCollider2D> ();
		if (circleCollider2D != null) {
			return(ColliderType.Circle);
		}

		CapsuleCollider2D capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D> ();
		if (capsuleCollider2D != null) {
			return(ColliderType.Capsule);
		}

		return(ColliderType.None);
	}

		static public Polygon2D CreateFromEdgeCollider(EdgeCollider2D edgeCollider) {
		Polygon2D newPolygon = new Polygon2D ();
		if (edgeCollider != null) {
			foreach (Vector2 p in edgeCollider.points) {
				newPolygon.AddPoint (p + edgeCollider.offset);
			}
			//newPolygon.AddPoint (edgeCollider.points[0] + edgeCollider.offset);
		}
		return(newPolygon);
	}

	static public Polygon2D CreateFromCircleCollider(CircleCollider2D circleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D ();

		float size = circleCollider.radius;
		float i = 0;

		while (i < 360) {
			newPolygon.AddPoint (new Vector2(Mathf.Cos (i * Mathf.Deg2Rad) * size, Mathf.Sin (i * Mathf.Deg2Rad) * size) + circleCollider.offset);
			i += 360f / (float)pointsCount;
		}

		return(newPolygon);
	}

	static public Polygon2D CreateFromBoxCollider(BoxCollider2D boxCollider) {
		Polygon2D newPolygon = new Polygon2D();

		Vector2 size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

		newPolygon.AddPoint (new Vector2(-size.x, -size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(-size.x, size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(size.x, size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(size.x, -size.y) + boxCollider.offset);

		return(newPolygon);
	}

	static public Polygon2D CreateFromCapsuleCollider(CapsuleCollider2D capsuleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D();

		Vector2 size = new Vector2(capsuleCollider.size.x / 2, capsuleCollider.size.y / 2);
		float offset = 0;
		float i = 0;

		switch (capsuleCollider.direction) {
		case CapsuleDirection2D.Vertical:
			float sizeXY = (capsuleCollider.transform.localScale.x / capsuleCollider.transform.localScale.y);
			size.x *= sizeXY;
			i = 0;

			if (capsuleCollider.size.x < capsuleCollider.size.y) 
				offset = (capsuleCollider.size.y - capsuleCollider.size.x) / 2;

			while (i < 180) {
				Vector2 v = new Vector2 (Mathf.Cos (i * Mathf.Deg2Rad) * size.x, offset + Mathf.Sin (i * Mathf.Deg2Rad) * size.x);
				newPolygon.AddPoint (v + capsuleCollider.offset);
				i += 360f / (float)pointsCount;
			}

			while (i < 360) {
				Vector2 v = new Vector2 (Mathf.Cos (i * Mathf.Deg2Rad) * size.x, -offset + Mathf.Sin (i * Mathf.Deg2Rad) * size.x);
				newPolygon.AddPoint (v + capsuleCollider.offset);
				i += 360f / (float)pointsCount;
			}
			break;

		case CapsuleDirection2D.Horizontal:
			float sizeYX = (capsuleCollider.transform.localScale.y / capsuleCollider.transform.localScale.x);
			size.x *= sizeYX; // not size.y?
			i = -90;

			if (capsuleCollider.size.y < capsuleCollider.size.x) 
				offset = (capsuleCollider.size.x - capsuleCollider.size.y) / 2;

			while (i < 90) {
				Vector2 v = new Vector2 (offset + Mathf.Cos (i * Mathf.Deg2Rad) * size.y, Mathf.Sin (i * Mathf.Deg2Rad) * size.y);
				newPolygon.AddPoint (v + capsuleCollider.offset);
				i += 360f / (float)pointsCount;
			}

			while (i < 270) {
				Vector2 v = new Vector2 (-offset + Mathf.Cos (i * Mathf.Deg2Rad) * size.y, Mathf.Sin (i * Mathf.Deg2Rad) * size.y);
				newPolygon.AddPoint (v + capsuleCollider.offset);
				i += 360f / (float)pointsCount;
			}
			break;
		}

		return(newPolygon);
	}

	// Capsule Missing
	static public Polygon2D Create(PolygonType type, float size = 1f) {
		Polygon2D newPolygon = new Polygon2D();

		switch (type) {

			case PolygonType.Pentagon:
				newPolygon.AddPoint (0f * size, 1f * size);
				newPolygon.AddPoint (-0.9510565f * size, 0.309017f * size);
				newPolygon.AddPoint (-0.5877852f * size, -0.8090171f * size);
				newPolygon.AddPoint (0.5877854f * size, -0.8090169f * size);
				newPolygon.AddPoint (0.9510565f * size, 0.3090171f * size);
				break;

			case PolygonType.Rectangle:
				newPolygon.AddPoint (-size, -size);
				newPolygon.AddPoint (size, -size);
				newPolygon.AddPoint (size, size);
				newPolygon.AddPoint (-size, size);
				break;

			case PolygonType.Circle:
				float i = 0;

				float cycle = 360f / (float)defaultCircleVerticesCount;

				while (i < 360 ) {
					newPolygon.AddPoint (Mathf.Cos (i * Mathf.Deg2Rad) * size, Mathf.Sin (i * Mathf.Deg2Rad) * size);
					i += cycle;
				}
				break;

			case PolygonType.Hexagon:
				for (int s = 1; s < 360; s = s + 60)
					newPolygon.AddPoint (Mathf.Cos (s * Mathf.Deg2Rad) * size, Mathf.Sin (s * Mathf.Deg2Rad) * size);

				break;

			case PolygonType.Octagon:
				for (int s = 1; s < 360; s = s + 40)
					newPolygon.AddPoint (Mathf.Cos (s * Mathf.Deg2Rad) * size, Mathf.Sin (s * Mathf.Deg2Rad) * size);

				break;
		}

		return(newPolygon);
	}

}

public class Polygon2DList : Polygon2DHelper {

	// Get List Of Polygons from Collider (Usually Used Before Creating Slicer2D Object)
	static public List<Polygon2D> CreateFromPolygonColliderToWorldSpace(PolygonCollider2D collider) {
		List<Polygon2D> result = new List<Polygon2D> ();

		if (collider != null && collider.pathCount > 0) {
			Polygon2D newPolygon = new Polygon2D ();

			foreach (Vector2 p in collider.GetPath (0)) {
				newPolygon.AddPoint (p + collider.offset);
			}
			
			newPolygon = newPolygon.ToWorldSpace(collider.transform);

			result.Add (newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Polygon2D hole = new Polygon2D ();
				foreach (Vector2 p in collider.GetPath (i)) {
					hole.AddPoint (p + collider.offset);
				}

				hole = hole.ToWorldSpace(collider.transform);

				if (newPolygon.PolyInPoly (hole) == true) {
					newPolygon.AddHole(hole);
				} else {
					result.Add(hole);
				}
			}
		}
		return(result);
	}

	static public List<Polygon2D> CreateFromPolygonColliderToLocalSpace(PolygonCollider2D collider) {
		List<Polygon2D> result = new List<Polygon2D>();

		if (collider != null && collider.pathCount > 0) {
			Polygon2D newPolygon = new Polygon2D ();

			foreach (Vector2 p in collider.GetPath (0)) {
				newPolygon.AddPoint (p + collider.offset);
			}

			result.Add(newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Polygon2D hole = new Polygon2D ();
				foreach (Vector2 p in collider.GetPath (i)) {
					hole.AddPoint (p + collider.offset);
				}

				if (newPolygon.PolyInPoly (hole) == true) {
					newPolygon.AddHole (hole);
				} else {
					result.Add(hole);
				}
			}
		}
		return(result);
	}

	// Slower CreateFromCollider
	public static List<Polygon2D> CreateFromGameObject(GameObject gameObject) {
		List<Polygon2D> result = new List<Polygon2D>();
		
		foreach(Collider2D c in gameObject.GetComponents<Collider2D> ()) {
			System.Type type = c.GetType();

			if (type == typeof(BoxCollider2D)) {
				BoxCollider2D boxCollider2D = (BoxCollider2D)c;
				

				result.Add(CreateFromBoxCollider(boxCollider2D));
			}

			if (type == typeof(CircleCollider2D)) {
				CircleCollider2D circleCollider2D = (CircleCollider2D)c;

				result.Add(CreateFromCircleCollider(circleCollider2D));
			}

			if (type == typeof(CapsuleCollider2D)) {
				CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)c;

				result.Add(CreateFromCapsuleCollider(capsuleCollider2D));
			}

			if (type == typeof(EdgeCollider2D)) {
				EdgeCollider2D edgeCollider2D = (EdgeCollider2D)c;

				result.Add(CreateFromEdgeCollider(edgeCollider2D));
			}

			if (type == typeof(PolygonCollider2D)) {
				PolygonCollider2D polygonCollider2D = (PolygonCollider2D)c;

				List<Polygon2D> polygonColliders = CreateFromPolygonColliderToLocalSpace(polygonCollider2D);

				foreach(Polygon2D poly in polygonColliders) {
					result.Add(poly);
				}

			}
		}

		foreach(Polygon2D poly in result) {
			poly.Normalize();
		}

		return(result);
	}
}
