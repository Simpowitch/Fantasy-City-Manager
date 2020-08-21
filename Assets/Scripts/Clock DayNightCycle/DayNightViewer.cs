using UnityEngine;
using UnityEngine.UI;

public class DayNightViewer : MonoBehaviour
{
    public Image image;

    public Color[] dayFilters = new Color[DayNightSystem.PARTSOFTHEDAY];

    private void OnEnable() => DayNightSystem.OnPartOfTheDayChanged += PartOfTheDayChanged;

    private void OnDisable() => DayNightSystem.OnPartOfTheDayChanged -= PartOfTheDayChanged;

    private void PartOfTheDayChanged(DayNightSystem.PartOfTheDay partOfTheDay) => image.color = dayFilters[(int)partOfTheDay];
}
