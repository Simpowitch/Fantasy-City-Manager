using UnityEngine;

public class GoodsProducer : Workplace
{
    [Header("Goods producer")]
    [SerializeField] CityResource.Type produce = CityResource.Type.Food;
    public CityResource.Type Produce { get; private set; }
    public int producePerEmployee = 1;

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        Produce = produce;
    }

    protected override void EndWorkingDay()
    {
        city.cityStats.GetResource(Produce).Value += producePerEmployee * EmployeesAtSite.Count;
        base.EndWorkingDay();
    }
}
