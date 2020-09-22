using UnityEngine;
[System.Serializable]
public class Need
{
    public delegate void NeedHandler();
    public event NeedHandler OnNeedValuesChanged;

    const float MINVALUE = 0;
    const float MAXVALUE = 1;
    const float LOWVALUE = 0.25f;
    const float HIGHVALUE = 0.75f;

    public enum NeedType
    {
        Energy,
        Hunger,
        Recreation,
        Social
    }

    public enum NeedState
    {
        Low,
        Medium,
        High
    }

    public string Title { get; private set; }
    public NeedType Type { get; private set; }
    float needDecreasePerHour;
    public NeedState State { get; private set; }

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
                SetState();
            }
        }
    }

    public Need(NeedType needType, float needDecreasePerHour, float startValue)
    {
        Type = needType;
        this.Title = needType.ToString();
        this.needDecreasePerHour = needDecreasePerHour;
        this.CurrentValue = startValue;

        Clock.OnHourChanged += HourProgressed;
    }

    public void Satisfy() => CurrentValue = MAXVALUE;

    void HourProgressed(int newHour) => CurrentValue -= needDecreasePerHour;

    void SetState()
    {
        if (CurrentValue <= LOWVALUE)
            State = NeedState.Low;
        else if (currentValue >= HIGHVALUE)
            State = NeedState.High;
        else
            State = NeedState.Medium;
    }


    [System.Serializable]
    public struct NeedProvision
    {
        public enum Type { Energy, Hunger, Recreation, Social }
        public Type type;
        public float value;
    }
}