using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Date : MonoBehaviour
{
    [SerializeField] Text dateTextField = null;

    public delegate void DayChangeHandler(int newDay);
    public static event DayChangeHandler OndayChanged;
    public UnityEvent OnNewDay;

    [SerializeField] string[] dayNames = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    private int day = 1;
    public int Day 
    {
        get => day;
        set
        {
            day = value;
            OnNewDay?.Invoke();
            OndayChanged?.Invoke(value);
            Print();
        }
    }

    private void Start()
    {
        Print();
    }

    public void Print()
    {
        string dayName = dayNames[(day - 1) % dayNames.Length];
        dateTextField.text = $"Day {day} - {dayName}";
    }
}
