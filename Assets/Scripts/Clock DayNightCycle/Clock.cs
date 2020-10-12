using UnityEngine;

public class Clock : MonoBehaviour
{
    public delegate void TimeValueChangeHandler(int newValue);
    public static event TimeValueChangeHandler OnMinuteChanged;
    public static event TimeValueChangeHandler OnHourChanged;
    public static event TimeValueChangeHandler OnDayChanged;

    public delegate void TimeSetHandler(int day, int hour, int minute);
    public static event TimeSetHandler OnTimeSet;

    public delegate void TimeHandler(string newTime);
    public static event TimeHandler OnTimeChanged;

    public const int HOURSPERDAY = 24;
    public const int MINUTESPERHOUR = 60;

    [Header("Settings")]
    [SerializeField] float realworldSecondsPerGameMinute = 0.5f;
    [SerializeField] bool clockOn = false;

    [Header("StartTime")]
    [SerializeField] int startMinute = 0;
    [SerializeField] int startHour = 6;
    [SerializeField] int startDay = 1;

    float secondsTimer; //Used with delta time

    int minute;
    public int Minute
    {
        get => minute;
        set
        {
            minute = value;
            if (minute >= MINUTESPERHOUR)
            {
                minute = 0;
                Hour++;
            }
            OnMinuteChanged?.Invoke(minute);
            OnTimeChanged?.Invoke(GetClockStatus());
        }
    }
    int hour;
    public int Hour
    {
        get => hour;
        set
        {
            hour = value;
            if (hour >= HOURSPERDAY)
            {
                hour = 0;
                day++;
                OnDayChanged?.Invoke(day);
            }
            OnHourChanged?.Invoke(hour);
        }
    }
    int day; //Not part of a clock, but useful for tracking the number of days gone by

    private void Start()
    {
        SetValues(startDay, startHour, startMinute);
    }

    public void SetValues(int day, int hour, int minute)
    {
        this.day = day;
        this.Hour = hour;
        this.Minute = minute;
        OnTimeSet?.Invoke(this.day, Hour, Minute);
    }

    //Starting and stopping the clock
    public void StartStopClock(bool start)
    {
        clockOn = start;
    }

    private void Update()
    {
        if (!clockOn)
        {
            return;
        }

        secondsTimer += Time.deltaTime;

        if (secondsTimer >= realworldSecondsPerGameMinute)
        {
            secondsTimer -= realworldSecondsPerGameMinute;
            Minute++;
        }
    }

    private string GetClockStatus() => string.Format("{0:00}:{1:00}", hour, minute);

    public static bool IsTimeBetween(int a, int b, int current)
    {
        if (a < b)
            return current >= a && current <= b;
        else if (a > b)
            return current <= a && current >= b;
        else
            return false;
    }
}
