using UnityEngine;
using UnityEngine.UI;

public class ClockViewer : MonoBehaviour
{
    public Text clockText;

    private void OnEnable() => Clock.OnTimeChanged += UpdateTime;

    private void OnDisable() => Clock.OnTimeChanged -= UpdateTime;

    private void UpdateTime(string newClockText) => clockText.text = newClockText;
}
