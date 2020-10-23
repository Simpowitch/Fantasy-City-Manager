using UnityEngine;
using UnityEngine.UI;

public class CityStatsViewer : MonoBehaviour
{
    [Header("Static references")]
    [SerializeField] Text population = null;
    [SerializeField] Text visitors = null;
    [SerializeField] Text gold = null, wood = null, stone = null, iron = null, food = null;
    [SerializeField] City city = null;

    private void OnEnable()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        city.cityStats.OnCityStatsChanged += PrintCityStats;
    }

    private void OnDisable()
    {
        city.cityStats.OnCityStatsChanged -= PrintCityStats;
    }

    void PrintCityStats(CityStats cityStats)
    {
        CityResourceGroup inventory = cityStats.Inventory;

        ChangeGoldText(inventory.GetCityResourceOfType(CityResource.Type.Gold).Value);
        ChangeWoodText(inventory.GetCityResourceOfType(CityResource.Type.Wood).Value);
        ChangeStoneText(inventory.GetCityResourceOfType(CityResource.Type.Stone).Value);
        ChangeIronText(inventory.GetCityResourceOfType(CityResource.Type.Iron).Value);
        ChangeFoodText(inventory.GetCityResourceOfType(CityResource.Type.Food).Value);

        ChangePopulationText(cityStats.Population, cityStats.PopulationCapacity);
        ChangeVisitorsText(cityStats.Visitors);
    }

    void ChangeGoldText(int newValue) => gold.text = newValue.ToString();
    void ChangeWoodText(int newValue) => wood.text = newValue.ToString();
    void ChangeStoneText(int newValue) => stone.text = newValue.ToString();
    void ChangeIronText(int newValue) => iron.text = newValue.ToString();
    void ChangeFoodText(int newValue) => food.text = newValue.ToString();

    void ChangePopulationText(int citizens, int populationCapacity) => population.text = $"{citizens} / {populationCapacity}";
    void ChangeVisitorsText(int newValue) => visitors.text = newValue.ToString();
}
