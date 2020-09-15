using UnityEngine;
[System.Serializable]
public class Need
{
    public delegate void NeedHandler();
    public event NeedHandler OnNeedValuesChanged;

    const float MINVALUE = 0;
    const float MAXVALUE = 1;

    public string Title { get; private set; }
    float needDecreasePerHour;


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
        }
    }

    public Need(string title, float needDecreasePerHour, float startValue)
    {
        this.Title = title;
        this.needDecreasePerHour = needDecreasePerHour;
        this.currentValue = startValue;

        Clock.OnHourChanged += HourProgressed;
    }

    void HourProgressed(int newHour) => CurrentValue -= needDecreasePerHour;

    [System.Serializable]
    public struct NeedProvision
    {
        public enum Type { Energy, Hunger, Recreation, Social }
        public Type type;
        public float value;
    }
}