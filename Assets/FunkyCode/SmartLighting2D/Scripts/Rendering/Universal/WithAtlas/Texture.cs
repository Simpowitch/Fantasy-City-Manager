using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Universal.WithAtlas {

	public class Texture : Base {

        // UV RECT (x, y, w, h)
        static public void Draw(Vector2 pos, Vector2 size, Rect uv, float rot, float z) {
            rot = rot * Mathf.Deg2Rad + Mathf.PI;

            float rectAngle = Mathf.Atan2(size.y, size.x);
            float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

            Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
            Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
            Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
            Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
            
            GL.TexCoord3 (uv.x, uv.y, 0);
			GL.Vertex3 (v1.x, v1.y, z);

			GL.TexCoord3 (uv.x, uv.height, 0);
			GL.Vertex3 (v2.x, v2.y, z);

			GL.TexCoord3 (uv.width, uv.height, 0);
			GL.Vertex3 (v3.x, v3.y, z);

			GL.TexCoord3 (uv.width, uv.height, 0);
			GL.Vertex3 (v3.x, v3.y, z);

			GL.TexCoord3 (uv.x, uv.height, 0);
			GL.Vertex3 (v4.x, v4.y, z);

			GL.TexCoord3 (uv.x, uv.y, 0);
			GL.Vertex3 (v1.x, v1.y, z);

        }
    }
}