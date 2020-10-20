using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightCycleBuffer
{
    public Gradient gradient = new Gradient();
}

[System.Serializable]
public class LightDayProperties
{
    [Range(0, 360)]
    public float shadowOffset = 0;

    public AnimationCurve shadowHeight = new AnimationCurve();

    public AnimationCurve shadowAlpha = new AnimationCurve();
}

[ExecuteInEditMode]
public class LightCycle : MonoBehaviour
{
    [Range(0, 360)]
    public float time = 0;

    public LightDayProperties dayProperties = new LightDayProperties();

    public LightCycleBuffer[] nightProperties = new LightCycleBuffer[1];

    public Clock clock;
    public bool useClockTime = true;

    private void Update()
    {
        if (useClockTime)
            time = clock.GetDayPercentage() * 360;
    }

    void LateUpdate()
    {
        LightingSettings.BufferPresetList bufferPresets = Lighting2D.Profile.bufferPresets;

        if (bufferPresets == null)
        {
            return;
        }


        float step = ((float)time) / 360;



        // Day Lighting Properties
        float height = dayProperties.shadowHeight.Evaluate(step);
        float alpha = dayProperties.shadowAlpha.Evaluate(step);

        if (height < 0.01f)
        {
            height = 0.01f;
        }

        if (alpha < 0)
        {
            alpha = 0;
        }

        Lighting2D.dayLightingSettings.height = height;
        Lighting2D.dayLightingSettings.alpha = alpha;
        Lighting2D.dayLightingSettings.direction = time + dayProperties.shadowOffset;




        // Dynamic Properties
        for (int i = 0; i < nightProperties.Length; i++)
        {
            if (i >= bufferPresets.list.Length)
            {
                return;
            }

            LightCycleBuffer buffer = nightProperties[i];
            Color color = buffer.gradient.Evaluate(step);

            LightingSettings.BufferPreset bufferPreset = bufferPresets.list[i];
            bufferPreset.darknessColor = color;
        }
    }
}
