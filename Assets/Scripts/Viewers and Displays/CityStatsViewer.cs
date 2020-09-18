using UnityEngine;
using UnityEngine.UI;

public class CityStatsViewer : MonoBehaviour
{
    [Header("Static references")]
    [SerializeField] Text population = null;
    [SerializeField] Text visitors = null;
    [SerializeField] Text gold = null, wood = null, stone = null, food = null;
    [SerializeField] Player player = null;
    City city;

    private void OnEnable()
    {
        if (city != null)
        {
            Subscribe(); 
        }
    }

    private void Subscribe()
    {
        city.cityStats.Gold.OnValueChanged += ChangeGoldText;
        city.cityStats.Wood.OnValueChanged += ChangeWoodText;
        city.cityStats.Stone.OnValueChanged += ChangeStoneText;
        city.cityStats.Food.OnValueChanged += ChangeFoodText;

        city.cityStats.OnPopulationChanged += ChangePopulationText;
        city.cityStats.OnVisitorsChanged += ChangeVisitorsText;

        city.cityStats.SendValuesToListeners();
    }

    private void OnDisable()
    {
        city.cityStats.Gold.OnValueChanged -= ChangeGoldText;
        city.cityStats.Wood.OnValueChanged -= ChangeWoodText;
        city.cityStats.Stone.OnValueChanged -= ChangeStoneText;
        city.cityStats.Food.OnValueChanged -= ChangeFoodText;

        city.cityStats.OnPopulationChanged -= ChangePopulationText;
        city.cityStats.OnVisitorsChanged -= ChangeVisitorsText;
    }

    private void Awake()
    {
        player.OnActiveCityChanged += SetCity;
    }

    private void SetCity(City city)
    {
        this.city = city;
        Subscribe();
    }


    void ChangeGoldText(int newValue) => gold.text = newValue.ToString();
    void ChangeWoodText(int newValue) => wood.text = newValue.ToString();
    void ChangeStoneText(int newValue) => stone.text = newValue.ToString();
    void ChangeFoodText(int newValue) => food.text = newValue.ToString();

    void ChangePopulationText(int newValue) => population.text = newValue.ToString();
    void ChangeVisitorsText(int newValue) => visitors.text = newValue.ToString();
}
