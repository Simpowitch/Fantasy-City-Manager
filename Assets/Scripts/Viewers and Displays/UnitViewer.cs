using TMPro;
using UnityEngine;

public class UnitViewer : MonoBehaviour
{
    [Header("Unit info")]
    [SerializeField] GameObject mainPanel = null;
    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] TextMeshProUGUI professionText = null;

    [SerializeField] GameObject energy = null, hunger = null, recreation = null, social = null;
    [Header("Titles")]
    [SerializeField] TextMeshProUGUI happinessTitle = null;
    [SerializeField] TextMeshProUGUI energyTitle = null, hungerTitle = null, recreationTitle = null, socialTitle = null;
    [Header("Bars")]
    [SerializeField] Bar happinessBar = null;
    [SerializeField] Bar energyBar = null, hungerBar = null, recreationBar = null, socialBar = null;

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

        happinessTitle.text = "Happiness";
        happinessBar.SetNewValues(unit.Happiness);

        if (unit.Energy != null)
        {
            energy.SetActive(true);
            energyTitle.text = unit.Energy.Title;
            energyBar.SetNewValues(unit.Energy.CurrentValue);
        }
        else
            energy.SetActive(false);


        if (unit.Hunger != null)
        {
            hunger.SetActive(true);
            hungerTitle.text = unit.Hunger.Title;
            hungerBar.SetNewValues(unit.Hunger.CurrentValue);
        }
        else
            hunger.SetActive(false);

        if (unit.Recreation != null)
        {
            recreation.SetActive(true);
            recreationTitle.text = unit.Recreation.Title;
            recreationBar.SetNewValues(unit.Recreation.CurrentValue);
        }
        else
            recreation.SetActive(false);

        if (unit.Social != null)
        {
            social.SetActive(true);
            socialTitle.text = unit.Social.Title;
            socialBar.SetNewValues(unit.Social.CurrentValue);
        }
        else
            social.SetActive(false);
    }

    public void SubscribeToChanges(Unit unit, bool state)
    {
        if (state)
        {
            unit.OnUnitInfoChanged += ShowUnit;
        }
        else
        {
            unit.OnUnitInfoChanged -= ShowUnit;
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
