using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DemoCollisionEvents : MonoBehaviour {
    static private Texture pointTexture;

    public int percentage = 0;
    
    LightCollision2D collision2DInfo = new LightCollision2D();
    LightingCollider2D Collider;

    static private Texture GetPointTexture() {
		if (pointTexture == null) {
			pointTexture = Resources.Load<Texture>("Textures/dot");
		}
		return(pointTexture);
	}

    void Start() {
        Collider = GetComponent<LightingCollider2D>();
        Collider.AddEvent(collisionEvent);

        collision2DInfo.lightingEventState = LightingEventState.None;
    }

    void collisionEvent(LightCollision2D collision) {
        if (collision.pointsColliding != null) {
            if (collision2DInfo.lightingEventState == LightingEventState.None) {
                collision2DInfo = collision;

            } else {
                if (collision.pointsColliding.Count >= collision2DInfo.pointsColliding.Count) {
                    collision2DInfo = collision;
                } else if (collision2DInfo.lightSource == collision.lightSource) {
                    collision2DInfo = collision;
                }
            }

        } else {
            collision2DInfo.lightingEventState = LightingEventState.None;
        }
    }

    void Update() {
        percentage = 0;

        if (collision2DInfo.lightingEventState != LightingEventState.None) {
            if (collision2DInfo.pointsColliding != null) {
                //Polygon2D localPoly = Collider.shape.GetPolygonsLocal()[0];
                //percentage = (int)(((float)collision2DInfo.pointsColliding.Count / localPoly.pointsList.Count) * 100);
            }
        }

        collision2DInfo.lightingEventState = LightingEventState.None;
    }

    void OnGUI() {
        if (Camera.main == null) {
            return;
        }
        
        Vector2 middlePoint = Camera.main.WorldToScreenPoint(transform.position);

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(middlePoint.x - 50, Screen.height - middlePoint.y - 50, 100, 100), percentage.ToString() + "%");

        if (collision2DInfo.lightingEventState == LightingEventState.None) {
            return;
        }

        foreach(Vector2 point in collision2DInfo.pointsColliding) {
            Vector2 pos = collision2DInfo.lightSource.transform.position;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(point + pos);
            //float distance = Vector2.Distance(point.ToVector2(), collision2DInfo.lightSource.transform.position);

            GUI.DrawTexture(new Rect(screenPoint.x - 5, Screen.height - screenPoint.y - 5, 10, 10), GetPointTexture());
            //GUI.Label(new Rect(screenPoint.x - 5 - 50, Screen.height - screenPoint.y - 5 - 50, 100, 100), distance.ToString());
        }
    }
}
