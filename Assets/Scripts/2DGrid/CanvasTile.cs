using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTile : MonoBehaviour
{
    public enum SelectionFillState { Off, Allowed, Disallowed};
    public enum ZoneFillState { Off, Zonable, Zoned };

    public Image selectionFill;
    public Image zoneFill;
    public Image outline;
    public Color allowedColor;
    public Color disallowedColor;
    public Color zonableColor;
    public Color zonedColor;

    private void Start()
    {
        SetSelectionFillState(SelectionFillState.Off);
        SetZoneFillState(ZoneFillState.Off);
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

    public void SetZoneFillState(ZoneFillState state)
    {
        zoneFill.enabled = state != ZoneFillState.Off;
        switch (state)
        {
            case ZoneFillState.Off:
                break;
            case ZoneFillState.Zonable:
                zoneFill.color = zonableColor;
                break;
            case ZoneFillState.Zoned:
                zoneFill.color = zonedColor;
                break;
        }
    }

    public void SetGridOutline(bool status) => outline.enabled = status;
}
