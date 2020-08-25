using TMPro;
using UnityEngine;

public class UnitViewer : MonoBehaviour
{
    [SerializeField] GameObject mainPanel = null;
    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] TextMeshProUGUI professionText = null;
    [SerializeField] TextMeshProUGUI moodExplainText = null;
    [SerializeField] Bar mood = null;
    [SerializeField] Bar food = null;
    [SerializeField] Bar recreation = null;

    Unit lastViewedUnit;

    public void ShowCitizen(Citizen citizen)
    {
        if (lastViewedUnit)
            lastViewedUnit.UnsubrscibeFromViewer(this);
        ActivatePanel(citizen != null);
        if (citizen)
        {
            nameText.text = citizen.UnitName;
            professionText.text = $"Profession: {citizen.GetProfession()}";
            moodExplainText.text = citizen.GetMoodExplanation();
            mood.SetNewValues(citizen.Mood.CurrentValue);
            food.SetNewValues(citizen.Food.CurrentValue);
            recreation.SetNewValues(citizen.Recreation.CurrentValue);

            lastViewedUnit = citizen;
            Subscribe(citizen, true);
        }
    }


    public void ShowVisitor(Visitor visitor)
    {

    }

    public void Hide()
    {
        if (lastViewedUnit)
            lastViewedUnit.UnsubrscibeFromViewer(this);
        ActivatePanel(false);
    }

    public void Subscribe(Citizen citizen, bool state)
    {
        if (state)
        {
            citizen.Mood.OnValueChanged += UpdateMoodBar;
            citizen.Food.OnValueChanged += UpdateFoodBar;
            citizen.Recreation.OnValueChanged += UpdateRecreationBar;
        }
        else
        {
            citizen.Mood.OnValueChanged -= UpdateMoodBar;
            citizen.Food.OnValueChanged -= UpdateFoodBar;
            citizen.Recreation.OnValueChanged -= UpdateRecreationBar;
        }
    }

    public void Subscribe(Visitor visitor, bool state)
    {

    }

    private void UpdateMoodBar(float newValue) => mood.SetNewValues(newValue);
    private void UpdateFoodBar(float newValue) => food.SetNewValues(newValue);
    private void UpdateRecreationBar(float newValue) => recreation.SetNewValues(newValue);

    private void ActivatePanel(bool state) => mainPanel.SetActive(state);
}
