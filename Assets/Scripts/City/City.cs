using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public float CellSize { get; } = 1;

    public CityStats cityStats = new CityStats();

    [Header("Setup")]
    int startGold = 100;
    int startWood = 25;
    int startStone = 25;
    int startIron = 10;
    int startFood = 100;
    int startCitizens = 5;

    private void OnEnable()
    {
        //DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
        Date.OndayChanged += SimulateBudget;
    }

    private void OnDisable()
    {
        //DayNightSystem.OnPartOfTheDayChanged -= PartOfDayChange;
        Date.OndayChanged -= SimulateBudget;
    }

    private void Awake()
    {
        Setup(startGold, startWood, startStone, startIron, startFood, startCitizens);
    }

    public void Setup(int gold, int wood, int stone, int iron, int food, int citizens)
    {
        cityStats.Setup(gold, wood, stone, iron, food);
        for (int i = 0; i < citizens; i++)
        {
            cityStats.AddCitizen(new Citizen());
        }
    }
   

    //void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay)
    //{
    //    switch (partOfDay)
    //    {
    //        case DayNightSystem.PartOfTheDay.Night:
    //            break;
    //        case DayNightSystem.PartOfTheDay.Morning:
    //            break;
    //        case DayNightSystem.PartOfTheDay.Day:
    //            break;
    //        case DayNightSystem.PartOfTheDay.Evening:
    //            SimulateBudget();
    //            break;
    //    }
    //}


    void SimulateBudget(int day)
    {
        //Incomes
        CityResourceGroup incomes = CityResourceGroup.CombineIncomes(cityStats.Districts);
        //Expenses
        CityResourceGroup upkeeps = CityResourceGroup.CombineUpkeeps(cityStats.Districts);

        CityResourceGroup change = incomes;
        change.Remove(upkeeps);

        cityStats.Inventory.Add(change);

        //cityStats.Inventory.Add(new CityResource(CityResource.Type.Gold, change));
    }
}