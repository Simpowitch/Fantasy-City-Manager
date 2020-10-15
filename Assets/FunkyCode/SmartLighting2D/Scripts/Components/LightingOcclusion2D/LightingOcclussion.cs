using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingOcclussion {
    VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();
    
    public List<List<Pair2D>> polygonPoints = new List<List<Pair2D>>();
    public List<List<Pair2D>> outlinePoints = new List<List<Pair2D>>();
    public List<List<DoublePair2D>> polygonPairs = new List<List<DoublePair2D>>();
    
    public LightingOcclussion(LightingOcclusionShape shape, float size) {
	
        spriteRenderer.sprite = shape.spriteCustomPhysicsShape.GetOriginalSprite();

        if (shape.spriteCustomPhysicsShape.GetSpriteRenderer() != null) {
            spriteRenderer.flipX = shape.spriteCustomPhysicsShape.GetSpriteRenderer().flipX;
            spriteRenderer.flipY = shape.spriteCustomPhysicsShape.GetSpriteRenderer().flipY;
        } else {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        polygonPoints.Clear();
       	outlinePoints.Clear();
        polygonPairs.Clear();

       	List<Polygon2D> polygons = null;

		switch(shape.shadowType) {
			case LightingOcclusion2D.ShadowType.Collider:
				polygons = new List<Polygon2D>();

				List<Polygon2D> polygons3 = shape.GetPolygonsLocal();

				foreach(Polygon2D p in polygons3) {
					Polygon2D poly = p.Copy();
				
					polygons.Add(poly);
				}
				

			break;

			case LightingOcclusion2D.ShadowType.SpriteCustomPhysicsShape:
				SpriteRenderer sRenderer = shape.spriteCustomPhysicsShape.GetSpriteRenderer();
			
				CustomPhysicsShape customShape = CustomPhysicsShapeManager.RequesCustomShape(sRenderer.sprite);

				List<Polygon2D> polygons2 = customShape.Get();

				polygons = new List<Polygon2D>();

				foreach(Polygon2D p in polygons2) {
					Polygon2D poly = p.Copy();
					polygons.Add(poly);
				}

			break;
		}

        if (polygons == null || polygons.Count < 1) {
            return;
        }

        foreach(Polygon2D polygon in polygons) {
            polygon.Normalize();
            
            polygonPoints.Add(Pair2D.GetList(polygon.pointsList));
            outlinePoints.Add(Pair2D.GetList(PreparePolygon(polygon, size).pointsList));
            polygonPairs.Add(DoublePair2D.GetList(polygon.pointsList));
        }
    }

    static public Polygon2D PreparePolygon(Polygon2D polygon, float size) {
		Polygon2D newPolygon = new Polygon2D();

		DoublePair2D pair = new DoublePair2D (null, null, null);;
		Vector2D pairA = Vector2D.Zero();
		Vector2D pairC = Vector2D.Zero();
		Vector2D vecA = Vector2D.Zero();
		Vector2D vecC = Vector2D.Zero();

		foreach (Vector2D pB in polygon.pointsList) {
			int indexB = polygon.pointsList.IndexOf (pB);

			int indexA = (indexB - 1);
			if (indexA < 0) {
				indexA += polygon.pointsList.Count;
			}

			int indexC = (indexB + 1);
			if (indexC >= polygon.pointsList.Count) {
				indexC -= polygon.pointsList.Count;
			}

			pair.A = polygon.pointsList[indexA];
			pair.B = pB;
			pair.C = polygon.pointsList[indexC];

			float rotA = (float)Vector2D.Atan2(pair.B, pair.A);
			float rotC = (float)Vector2D.Atan2(pair.B, pair.C);

			pairA.x = pair.A.x;
			pairA.y = pair.A.y;
			pairA.Push(rotA - Mathf.PI / 2, -size);

			pairC.x = pair.C.x;
			pairC.y = pair.C.y;
			pairC.Push(rotC + Mathf.PI / 2, -size);
			
			vecA.x = pair.B.x;
			vecA.y = pair.B.y;
			vecA.Push(rotA - Mathf.PI / 2, -size);
			vecA.Push(rotA, 110f);

			vecC.x = pair.B.x;
			vecC.y = pair.B.y;
			vecC.Push(rotC + Mathf.PI / 2, -size);
			vecC.Push(rotC, 110f);

			Vector2D result = Math2D.GetPointLineIntersectLine(new Pair2D(pairA, vecA), new Pair2D(pairC, vecC));

			if (result != null) {
				newPolygon.AddPoint(result);
			}
		}

		return(newPolygon);
	}   
}