using TMPro;
using UnityEngine;

public class UnitViewer : MonoBehaviour
{
    [SerializeField] GameObject mainPanel = null;
    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] TextMeshProUGUI professionText = null;
    [SerializeField] TextMeshProUGUI moodExplainText = null;
    [SerializeField] TextMeshProUGUI moodExplainValueText = null;


    [Header("Parent Objects")]
    [SerializeField] GameObject mood = null;
    [SerializeField] GameObject health = null, food = null, employment = null, recreation = null, faith = null, hygiene = null;
    [Header("Titles")]
    [SerializeField] TextMeshProUGUI moodTitle = null;
    [SerializeField] TextMeshProUGUI healthBarTitle = null, foodBarTitle = null, employmentBarTitle = null, recreationBarTitle = null, faithBarTitle = null, hygieneBarTitle = null;
    [Header("Bars")]
    [SerializeField] Bar moodBar = null;
    [SerializeField] Bar healthBar = null, foodBar = null, employmentBar = null, recreationBar = null, faithBar = null, hygieneBar = null;

    Unit lastViewedUnit = null;


    public void ShowUnit(Unit unit)
    {
        ActivatePanel(true);

        if (lastViewedUnit != null)
            SubscribeToChanges(lastViewedUnit, false);
        lastViewedUnit = unit;
        SubscribeToChanges(lastViewedUnit, true);

        nameText.text = unit.UnitName;
        professionText.text = unit.GetProfession();

        if (unit.Mood != null)
        {
            mood.SetActive(true);
            moodTitle.text = "Mood";
            moodBar.SetNewValues(unit.Mood.MoodValue, Mood.MAXVALUE, Mood.MINVALUE);
            moodExplainText.text = unit.Mood.MoodExplanation;
            moodExplainValueText.text = unit.Mood.MoodBuffExplanationValues;
        }
        else
        {
            mood.SetActive(false);
            moodExplainText.text = "";
            moodExplainValueText.text = "";
        }


        if (unit.Health != null)
        {
            health.SetActive(true);
            healthBarTitle.text = unit.Health.Title;
            healthBar.SetNewValues(unit.Health.CurrentValue);
        }
        else
            health.SetActive(false);

        if (unit.Food != null)
        {
            food.SetActive(true);
            foodBarTitle.text = unit.Food.Title;
            foodBar.SetNewValues(unit.Food.CurrentValue);
        }
        else
            food.SetActive(false);

        if (unit.Employment != null)
        {
            employment.SetActive(true);
            employmentBarTitle.text = unit.Employment.Title;
            employmentBar.SetNewValues(unit.Employment.CurrentValue);
        }
        else
            employment.SetActive(false);

        if (unit.Recreation != null)
        {
            recreation.SetActive(true);
            recreationBarTitle.text = unit.Recreation.Title;
            recreationBar.SetNewValues(unit.Recreation.CurrentValue);
        }
        else
            recreation.SetActive(false);

        if (unit.Faith != null)
        {
            faith.SetActive(true);
            faithBarTitle.text = unit.Faith.Title;
            faithBar.SetNewValues(unit.Faith.CurrentValue);
        }
        else
            faith.SetActive(false);

        if (unit.Hygiene != null)
        {
            hygiene.SetActive(true);
            hygieneBarTitle.text = unit.Hygiene.Title;
            hygieneBar.SetNewValues(unit.Hygiene.CurrentValue);
        }
        else
            hygiene.SetActive(false);
    }

    public void SubscribeToChanges(Unit unit, bool state)
    {
        if (state)
        {
            unit.MoodRecalculated += ShowUnit;
        }
        else
        {
            unit.MoodRecalculated -= ShowUnit;
        }
    }


    public void Hide()
    {
        if (lastViewedUnit)
            SubscribeToChanges(lastViewedUnit, false);
        ActivatePanel(false);
    }

    private void ActivatePanel(bool state) => mainPanel.SetActive(state);
}
