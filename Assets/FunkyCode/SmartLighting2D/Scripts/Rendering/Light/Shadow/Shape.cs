using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Shape : Base {

        public static void Draw(LightingBuffer2D buffer, LightingCollider2D id, float lightSizeSquared, float z) {
            if (id.InLightSource(buffer) == false) {
                return;
            }
            
            List<Polygon2D> polygons = id.shape.GetPolygonsWorld();

            if (polygons.Count < 1) {
                return;
            }
            
            Polygon.Draw(buffer, polygons, lightSizeSquared, z, -buffer.lightSource.transform2D.position, Vector2.one);
        }
    }
}