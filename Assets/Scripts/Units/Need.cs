using UnityEngine;

public struct Need
{
    public enum State { Critical, Low, Normal }
    public delegate void NeedValueHandler(float newValue);
    public event NeedValueHandler OnValueChanged;
    public delegate void NeedStateHandler(State newState);
    public event NeedStateHandler OnStateChanged;

    const float MINVALUE = 0;
    const float MAXVALUE = 1;

    public string name;
    public float needDecreasePerHour;
    public float criticalThreshold;
    public float lowThreshold;

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
                OnValueChanged?.Invoke(currentValue);
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
                OnStateChanged?.Invoke(value);
            }
            else
            {
                state = value;
            }
        }
    }

    public Need(string name, float needDecreasePerHour, float criticalThreshold, float lowThreshold, float startValue, NeedValueHandler OnNeedValueChanged, NeedStateHandler OnNeedStateChanged)
    {
        this.name = name;
        this.needDecreasePerHour = needDecreasePerHour;
        this.criticalThreshold = criticalThreshold;
        this.lowThreshold = lowThreshold;
        this.currentValue = startValue;
        this.state = State.Normal;
        OnValueChanged = OnNeedValueChanged;
        OnStateChanged = OnNeedStateChanged;

        Clock.OnHourChanged += HourProgressed;
    }

    private void SetState(float newValue)
    {
        if (newValue < lowThreshold)
        {
            if (newValue < criticalThreshold)
            {
                CurrentState = State.Critical;
            }
            else
            {
                CurrentState = State.Low;
            }
        }
        else
        {
            CurrentState = State.Normal;
        }
    }
    void HourProgressed(int newHour) => CurrentValue -= needDecreasePerHour;
}
