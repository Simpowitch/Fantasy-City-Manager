using TMPro;
using UnityEngine;

public class ObjectViewer : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] GameObject mainPanel = null;
    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] TextMeshProUGUI specialityText = null;
    [SerializeField] TextMeshProUGUI actionText = null;

    IViewable lastViewed;
    [SerializeField] TextMeshProUGUI primaryStatTitle = null;
    [SerializeField] Bar primaryStatBar = null;
    [SerializeField] GameObject[] needObjects = null;
    [SerializeField] TextMeshProUGUI[] needTitles = null;
    [SerializeField] Bar[] needBars = null;

    public void ShowViewable(IViewable viewable)
    {
        ActivatePanel(true);

        if (lastViewed != null)
            SubscribeToChanges(lastViewed, false);
        lastViewed = viewable;
        SubscribeToChanges(lastViewed, true);

        nameText.text = viewable.Name;
        specialityText.text = viewable.GetSpeciality();
        actionText.text = viewable.ActionDescription;

        primaryStatTitle.text = viewable.GetPrimaryStatName();
        primaryStatBar.SetNewValues(viewable.GetPrimaryStatValue());
        Need[] needs = viewable.GetNeeds();

        for (int i = 0; i < needObjects.Length; i++)
        {
            //Display all needs
            if (needs != null && i < needs.Length) //Need exists 
            {
                needObjects[i].SetActive(true);
                needTitles[i].text = needs[i].Title;
                needBars[i].SetNewValues(needs[i].CurrentValue);
            }
            else //Need objects are more than the needs
                needObjects[i].SetActive(false);
        }
    }

    public void SubscribeToChanges(IViewable viewable, bool state)
    {
        InfoChangeHandler handler = viewable.InfoChangeHandler;
        if (state)
            handler += ShowViewable;
        else
            handler -= ShowViewable;
        viewable.InfoChangeHandler = handler;
    }

    public void Hide()
    {
        if (lastViewed != null)
            SubscribeToChanges(lastViewed, false);
        ActivatePanel(false);
    }

    private void ActivatePanel(bool state) => mainPanel.SetActive(state);
}
