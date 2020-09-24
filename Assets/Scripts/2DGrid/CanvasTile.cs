using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTile : MonoBehaviour
{
    public enum SelectionFillState { Off, Allowed, Disallowed};

    public Image selectionFill;
    public Image outline;
    public Color allowedColor;
    public Color disallowedColor;
    public Text text;

    private void Start()
    {
        SetSelectionFillState(SelectionFillState.Off);
        SetGridOutline(false);
    }

    public void SetSelectionFillState(SelectionFillState state)
    {
        selectionFill.enabled = state != SelectionFillState.Off;
        switch (state)
        {
            case SelectionFillState.Off:
                break;
            case SelectionFillState.Allowed:
                selectionFill.color = allowedColor;
                break;
            case SelectionFillState.Disallowed:
                selectionFill.color = disallowedColor;
                break;
        }
    }

    public void SetGridOutline(bool status) => outline.enabled = status;

    public void SetText(string newText) => text.text = newText;
}
