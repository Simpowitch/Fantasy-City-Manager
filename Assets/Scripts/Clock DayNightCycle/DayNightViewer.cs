using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightViewer : MonoBehaviour
{
    public Light2D lightSource;

    public Color[] dayFilters = new Color[DayNightSystem.PARTSOFTHEDAY];

    private void OnEnable() => DayNightSystem.OnPartOfTheDayChanged += PartOfTheDayChanged;

    private void OnDisable() => DayNightSystem.OnPartOfTheDayChanged -= PartOfTheDayChanged;

    private void PartOfTheDayChanged(DayNightSystem.PartOfTheDay partOfTheDay) => lightSource.color = dayFilters[(int)partOfTheDay];
}
