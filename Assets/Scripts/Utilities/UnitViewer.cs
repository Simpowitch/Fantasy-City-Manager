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

    public void ShowUnit(Unit unit)
    {
        if (lastViewedUnit)
            SubscribeToViewedUnitNeeds(false);

        ActivatePanel(unit != null);
        if (unit)
        {
            nameText.text = unit.UnitName;
            professionText.text = $"Profession: {unit.GetProfession()}";
            moodExplainText.text = "";
            mood.SetNewValues(unit.Mood.CurrentValue);
            food.SetNewValues(unit.Food.CurrentValue);
            recreation.SetNewValues(unit.Recreation.CurrentValue);

            lastViewedUnit = unit;
            SubscribeToViewedUnitNeeds(true);
        }
    }

    private void SubscribeToViewedUnitNeeds(bool state)
    {
        if (state)
        {
            lastViewedUnit.Mood.OnValueChanged += UpdateMoodBar;
            lastViewedUnit.Food.OnValueChanged += UpdateFoodBar;
            lastViewedUnit.Recreation.OnValueChanged += UpdateRecreationBar;
        }
        else
        {
            lastViewedUnit.Mood.OnValueChanged -= UpdateMoodBar;
            lastViewedUnit.Food.OnValueChanged -= UpdateFoodBar;
            lastViewedUnit.Recreation.OnValueChanged -= UpdateRecreationBar;
        }
    }

    private void UpdateMoodBar(float newValue) => mood.SetNewValues(newValue);
    private void UpdateFoodBar(float newValue) => food.SetNewValues(newValue);
    private void UpdateRecreationBar(float newValue) => recreation.SetNewValues(newValue);




    public void ActivatePanel(bool state) => mainPanel.SetActive(state);
}
