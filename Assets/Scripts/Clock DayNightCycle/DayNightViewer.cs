using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightViewer : MonoBehaviour
{
    public Light2D lightSource;

    public Color[] dayFilters = new Color[DayNightSystem.PARTSOFTHEDAY];
    public float fluidChangeTime = 2f;
    private void OnEnable() => DayNightSystem.OnPartOfTheDayChanged += PartOfTheDayChanged;

    private void OnDisable() => DayNightSystem.OnPartOfTheDayChanged -= PartOfTheDayChanged;

    IEnumerator PerformColorChange(Color newColor, float animationTime)
    {
        Debug.Log("foo");
        float timer = 0;
        Color startCurrent = lightSource.color;
        float r1, g1, b1, a1;
        float r2, g2, b2, a2;

        r1 = startCurrent.r;
        g1 = startCurrent.g;
        b1 = startCurrent.b;
        a1 = startCurrent.a;

        r2 = newColor.r;
        g2 = newColor.g;
        b2 = newColor.b;
        a2 = newColor.a;

        while (timer < animationTime && startCurrent != newColor)
        {
            timer += Time.deltaTime;
            float t = timer / animationTime;

            float r3, g3, b3, a3;
            r3 = Mathf.Lerp(r1, r2, t);
            g3 = Mathf.Lerp(g1, g2, t);
            b3 = Mathf.Lerp(b1, b2, t);
            a3 = Mathf.Lerp(a1, a2, t);

            Color c = new Color(r3, g3, b3, a3);
            lightSource.color = c;
            yield return null;
        }
        Debug.Log("done");
        lightSource.color = newColor;
    }

    private void PartOfTheDayChanged(DayNightSystem.PartOfTheDay partOfTheDay)
    {
        if (fluidChangeTime > 0)
            StartCoroutine(PerformColorChange(dayFilters[(int)partOfTheDay], fluidChangeTime));
        else
            lightSource.color = dayFilters[(int)partOfTheDay];
    }
}
