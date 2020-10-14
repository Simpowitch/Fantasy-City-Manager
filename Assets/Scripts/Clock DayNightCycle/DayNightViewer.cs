using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightViewer : MonoBehaviour
{
    public Light2D lightSource;

    public Color[] dayFilters = new Color[DayNightSystem.PARTSOFTHEDAY];
    public float fluidChangeTime = 5f;

    int sunUpStart = 4;
    int sunUpEnd = 8;
    int sunDownStart = 18;
    int sunDownEnd = 22;
    int hoursBetweenStartAndEnd = 4;

    int sunHighHour = 12, hoursBetweenHighAndLow = 6;
    [SerializeField] float darknessAlphaMin = 0, darknessAlphaMax = 0.75f;
    [SerializeField] float sunAlphaMin = 0, sunAlphaMax = 0.5f;
    [SerializeField] float heightMin = 0, heightMax = 4;
    const int sunDayRotationDegrees = 360;
    float DegreesPerHour { get => sunDayRotationDegrees / Clock.HOURSPERDAY; }

    private void OnEnable()
    {
        Clock.OnHourChanged += HourChanged;
        DayNightSystem.OnPartOfTheDayChanged += PartOfTheDayChanged;
    }

    private void OnDisable()
    {
        Clock.OnHourChanged -= HourChanged;
        DayNightSystem.OnPartOfTheDayChanged -= PartOfTheDayChanged;
    }

    private void PartOfTheDayChanged(DayNightSystem.PartOfTheDay partOfTheDay)
    {
        if (fluidChangeTime > 0)
            StartCoroutine(PerformColorChange(dayFilters[(int)partOfTheDay], fluidChangeTime));
        else
            lightSource.color = dayFilters[(int)partOfTheDay];
    }

    IEnumerator PerformColorChange(Color newColor, float animationTime)
    {
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
        lightSource.color = newColor;
    }


    private void HourChanged(int newHour)
    {
        int hoursFromSunHigh = Mathf.Abs(newHour - sunHighHour);
        float sunPositionPercentage = (float) (hoursBetweenHighAndLow - hoursFromSunHigh) / hoursBetweenHighAndLow;

        float sunStrengthPercentage = 0;
        if (newHour >= sunUpStart && newHour < sunDownEnd) //Not in the night
        {
            if (newHour >= sunUpEnd && newHour < sunDownStart) //Middle of the day
                sunStrengthPercentage = 1;
            else
            {
                if (newHour >= sunUpStart && newHour <= sunUpEnd) //Morning
                {
                    int hoursToFull = Mathf.Abs(sunUpEnd - newHour);
                    sunStrengthPercentage = (float)(hoursBetweenStartAndEnd - hoursToFull) / hoursBetweenStartAndEnd;
                }
                else
                {
                    int hoursToDark = Mathf.Abs(sunDownEnd - newHour);
                    sunStrengthPercentage = 1 - (float)(hoursBetweenStartAndEnd - hoursToDark) / hoursBetweenStartAndEnd;
                }
            }
        }

        float newDirection = DegreesPerHour * newHour;
        float newAlpha = Mathf.Lerp(sunAlphaMin, sunAlphaMax, sunStrengthPercentage);
        float newHeight = Mathf.Lerp(heightMax, heightMin, sunPositionPercentage); //Reversed
        float newDarknessAlpha = Mathf.Lerp(darknessAlphaMax, darknessAlphaMin, sunStrengthPercentage); //Reversed

        Color newColor = Lighting2D.Profile.DarknessColor;
        newColor.a = newDarknessAlpha;
        Lighting2D.Profile.DarknessColor = newColor;

        if (hoursFromSunHigh > hoursBetweenHighAndLow) //Darkness
        {
            Lighting2D.Profile.dayLightingSettings.direction = 0;
            Lighting2D.Profile.dayLightingSettings.alpha = 0;
            Lighting2D.Profile.dayLightingSettings.height = 0;
        }
        else //Day
        {
            Lighting2D.Profile.dayLightingSettings.direction = newDirection;
            Lighting2D.Profile.dayLightingSettings.alpha = newAlpha;
            Lighting2D.Profile.dayLightingSettings.height = newHeight;
        }

        //if (hoursFromSunHigh > hoursBetweenHighAndLow) //Darkness
        //{
        //    StartCoroutine(PerformSunlightChange(0, 0, 0, 1f, fluidChangeTime));
        //}
        //else //Day
        //{
        //    StartCoroutine(PerformSunlightChange(newAngle, newAlpha, newHeight, 1f, fluidChangeTime));
        //}
    }

    IEnumerator PerformSunlightChange(float newDirection, float newAlpha, float newHeight, float timeBetweenUpdates, float animationTime)
    {
        float timer = 0;

        float startDirection = Lighting2D.Profile.dayLightingSettings.direction;
        float startAlpha = Lighting2D.Profile.dayLightingSettings.alpha;
        float startHeight = Lighting2D.Profile.dayLightingSettings.height;

        while (timer < animationTime)
        {
            timer += timeBetweenUpdates;
            float t = timer / animationTime;

            Lighting2D.Profile.dayLightingSettings.direction = Mathf.Lerp(startDirection, newDirection, t);
            Lighting2D.Profile.dayLightingSettings.alpha = Mathf.Lerp(startAlpha, newAlpha, t);
            Lighting2D.Profile.dayLightingSettings.height = Mathf.Lerp(startHeight, newHeight, t);
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
        Lighting2D.Profile.dayLightingSettings.direction = newDirection;
        Lighting2D.Profile.dayLightingSettings.alpha = newAlpha;
        Lighting2D.Profile.dayLightingSettings.height = newHeight;
    }
}
