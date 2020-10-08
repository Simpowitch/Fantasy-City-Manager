using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventHandling {
	public class Base {
		public static Vector2D vA = Vector2D.Zero(), vB = Vector2D.Zero();
		public static Vector2D vC = Vector2D.Zero(), vD = Vector2D.Zero();
		public static Polygon2D eventPoly = null;

		static public Polygon2D GetPolygon() {
			if (eventPoly == null) {
				eventPoly = new Polygon2D();
				eventPoly.AddPoint(Vector2D.Zero());
				eventPoly.AddPoint(Vector2D.Zero());
				eventPoly.AddPoint(Vector2D.Zero());
				eventPoly.AddPoint(Vector2D.Zero());
			}
			return(eventPoly);
		}
	}
}