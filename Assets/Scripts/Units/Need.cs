using UnityEngine;
[System.Serializable]
public class Need
{
    public enum State { Critical, Low, Normal, High }
    public delegate void NeedHandler();
    public event NeedHandler OnNeedValuesChanged;

    const float MINVALUE = 0;
    const float MAXVALUE = 1;

    public string Title { get; private set; }
    float needDecreasePerHour;
    float criticalThreshold;
    float lowThreshold;
    float highThreshold;

    public MoodBuff ActiveMoodBuff { get; private set; }

    MoodBuff CriticalMoodBuff { get; set; }
    MoodBuff LowMoodBuff { get; set; }
    MoodBuff NormalMoodBuff { get; set; }
    MoodBuff HighMoodBuff { get; set; }

    float currentValue;
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            float newValue = Mathf.Clamp(value, MINVALUE, MAXVALUE);
            if (currentValue != newValue)
            {
                currentValue = newValue;
                OnNeedValuesChanged?.Invoke();
            }
            else
            {
                currentValue = newValue;
            }
            SetState(newValue);
        }
    }

    State state;
    public State CurrentState
    {
        get => state;
        private set
        {
            if (state != value)
            {
                state = value;
                ChangeActiveMoodBuff(value);
            }
            else
            {
                state = value;
            }
        }
    }

    public Need(string title, float needDecreasePerHour, float criticalThreshold, float lowThreshold, float highThreshold, float startValue,
        MoodBuff critical, MoodBuff low, MoodBuff normal, MoodBuff high)
    {
        this.Title = title;
        this.needDecreasePerHour = needDecreasePerHour;
        this.criticalThreshold = criticalThreshold;
        this.lowThreshold = lowThreshold;
        this.highThreshold = highThreshold;
        this.currentValue = startValue;
        this.state = State.Normal;

        CriticalMoodBuff = critical;
        LowMoodBuff = low;
        NormalMoodBuff = normal;
        HighMoodBuff = high;

        SetState(startValue);

        Clock.OnHourChanged += HourProgressed;
    }

    private void SetState(float newValue)
    {
        if (newValue <= lowThreshold)
        {
            if (newValue <= criticalThreshold)
            {
                CurrentState = State.Critical;
            }
            else
            {
                CurrentState = State.Low;
            }
        }
        else if (newValue >= highThreshold)
        {
            CurrentState = State.High;
        }
        else
        {
            CurrentState = State.Normal;
        }
    }

    private void ChangeActiveMoodBuff(State state)
    {
        switch (state)
        {
            case State.Critical:
                ActiveMoodBuff = CriticalMoodBuff;
                break;
            case State.Low:
                ActiveMoodBuff = LowMoodBuff;
                break;
            case State.Normal:
                ActiveMoodBuff = NormalMoodBuff;
                break;
            case State.High:
                ActiveMoodBuff = HighMoodBuff;
                break;
        }
    }

    void HourProgressed(int newHour) => CurrentValue -= needDecreasePerHour;

    [System.Serializable]
    public struct NeedProvision
    {
        public enum Type { Healh, Food, Employment, Recreation, Faith, Hygiene }
        public Type type;
        public float value;
    }
}


