using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingSpriteRendererColor : MonoBehaviour {
    public LightingLayer nightLayer = LightingLayer.Layer1;
    public Color color;

    void Update() {
        foreach(LightingSpriteRenderer2D sprite in LightingSpriteRenderer2D.GetList()) {
            if (sprite.nightLayer == nightLayer) {
                sprite.color = color;
            }
        }
    }
}
