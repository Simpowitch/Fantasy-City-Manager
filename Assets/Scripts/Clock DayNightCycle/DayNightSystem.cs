using UnityEngine;
using UnityEngine.Events;

public class DayNightSystem : MonoBehaviour
{
    const int MORNINGHOUR = 6, DAYHOUR = 9, EVENINGHOUR = 18, NIGHTHOUR = 20;

    public delegate void PartOfTheDayChangeHandler(PartOfTheDay partOfTheDay);
    public static event PartOfTheDayChangeHandler OnPartOfTheDayChanged;

    public UnityEvent NightStarted, MorningStarted, DayStarted, EveningStarted;

    public enum PartOfTheDay { Night, Morning, Day, Evening }
    public const int PARTSOFTHEDAY = 4;


    PartOfTheDay partOfTheDay;
    public PartOfTheDay DayPart
    {
        get => partOfTheDay;
        private set
        {
            partOfTheDay = value;
            Debug.Log($"Part of the day changed to {partOfTheDay}");
            switch (value)
            {
                case PartOfTheDay.Night:
                    NightStarted?.Invoke();
                    break;
                case PartOfTheDay.Morning:
                    MorningStarted?.Invoke();
                    break;
                case PartOfTheDay.Day:
                    DayStarted?.Invoke();
                    break;
                case PartOfTheDay.Evening:
                    EveningStarted?.Invoke();
                    break;
            }
            OnPartOfTheDayChanged?.Invoke(value);
        }
    }

    private void OnEnable()
    {
        Clock.OnHourChanged += HourChanged;
        Clock.OnTimeSet += NewTimeSet;
    }

    private void OnDisable()
    {
        Clock.OnHourChanged -= HourChanged;
        Clock.OnTimeSet -= NewTimeSet;
    }


    private void NewTimeSet(int hour, int minute)
    {
        if (hour >= MORNINGHOUR && hour < DAYHOUR)
        {
            DayPart = PartOfTheDay.Morning;
        }
        else if (hour >= DAYHOUR && hour < EVENINGHOUR)
        {
            DayPart = PartOfTheDay.Day;
        }
        else if (hour >= EVENINGHOUR && hour < NIGHTHOUR)
        {
            DayPart = PartOfTheDay.Evening;
        }
        else
        {
            DayPart = PartOfTheDay.Night;
        }
    }

    private void HourChanged(int newHour)
    {
        switch (newHour)
        {
            case MORNINGHOUR:
                DayPart = PartOfTheDay.Morning;
                break;
            case DAYHOUR:
                DayPart = PartOfTheDay.Day;
                break;
            case EVENINGHOUR:
                DayPart = PartOfTheDay.Evening;
                break;
            case NIGHTHOUR:
                DayPart = PartOfTheDay.Night;
                break;
            default:
                break;
        }
    }
}
