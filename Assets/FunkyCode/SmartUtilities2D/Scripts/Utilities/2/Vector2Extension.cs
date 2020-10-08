using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{

    public static float Atan2(this Vector2 a, Vector2 b) {
		return(Mathf.Atan2 (a.y - b.y, a.x - b.x));
	}

    public static Vector2 Push(this Vector2 a, float direction, float distance) {
		a.x += Mathf.Cos(direction) * distance;
		a.y += Mathf.Sin(direction) * distance;

        return(a);
	}

	public static Vector2 RotToVec(this Vector2 a,float rotation, float distance) {
		a.x = Mathf.Cos(rotation) * distance;
		a.y = Mathf.Sin(rotation) * distance;

		return(a);
	}
	
	public static Vector2 RotToVec(this Vector2 a, float rotation) {
		a.x = Mathf.Cos(rotation);
		a.y = Mathf.Sin(rotation);

		return(a);
	}
}