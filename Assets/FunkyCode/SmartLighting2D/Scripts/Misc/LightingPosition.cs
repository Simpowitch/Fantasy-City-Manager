using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class LightingPosition
{
    public static Vector2 Get(Transform transform) {
        Vector3 position = transform.position;

		switch(Lighting2D.coreAxis) {
            case CoreAxis.XY:
                return(new Vector2(position.x, position.y));

            case CoreAxis.XZ:
				return(new Vector2(position.x, -position.z));
		}

        return(Vector2.zero);
    }

    public static Vector3 GetCamera(Camera camera) {
		Vector3 pos = camera.transform.position;
        float offset = camera.nearClipPlane + 0.1f;

		switch(Lighting2D.coreAxis) {
			case CoreAxis.XY:
				pos.z += offset;
			break;

			case CoreAxis.XZ:
				pos.y += offset;
			break;
		}
		
		return(pos);
	}	
}
