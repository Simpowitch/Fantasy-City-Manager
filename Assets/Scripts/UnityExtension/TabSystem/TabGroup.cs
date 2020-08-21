using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    TabButton selectedButton;
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();

        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        if (selectedButton == button)
        {
            return;
        }
        button.background.sprite = tabHover;
        ResetTabs();
    }

    public void OnTabExit(TabButton button)
    {
        if (selectedButton == button)
        {
            return;
        }
        button.background.sprite = tabIdle;
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedButton == button)
        {
            return;
        }
        if (selectedButton)
        {
            selectedButton.Deselect();
        }
        selectedButton = button; //Assign new
        selectedButton.Select();
        button.background.sprite = tabActive;

        ResetTabs();
    }

    public void ResetSelection()
    {
        if (selectedButton)
        {
            selectedButton.Deselect();
        }
        selectedButton = null;
        ResetTabs();
    }

    private void ResetTabs()
    {
        foreach (var tabButton in tabButtons)
        {
            if (selectedButton == tabButton)
            {
                continue;
            }
            tabButton.background.sprite = tabIdle;
        }
    }
}
