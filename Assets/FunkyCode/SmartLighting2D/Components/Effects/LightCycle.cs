using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightCycle : MonoBehaviour
{
    [Range(0, 360)]
    public float time = 0;

    [Range(0, 360)]
    public float shadowOffset = 0;

    public Gradient gradient = new Gradient();
    
    public AnimationCurve shadowHeight = new AnimationCurve();

    void LateUpdate() {
        float step = ((float)time) / 360;
        Color color = gradient.Evaluate(step);
        float height = shadowHeight.Evaluate(step);

        if (height < 0.01f) {
            height = 0.01f;
        }

        Lighting2D.dayLightingSettings.direction = time + shadowOffset;
        Lighting2D.dayLightingSettings.height = height;
        Lighting2D.darknessColor = color;
    }
}
