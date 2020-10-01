
public class CityGate : Workplace
{
    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.cityGates.Add(this);
    }
    protected override void UnitVisiting(Unit unitVisiting)
    {
        base.UnitVisiting(unitVisiting);
        if (unitVisiting is Visitor)
        {
            Visitor visitor = unitVisiting as Visitor;
            if (!visitor.PaidEntryToll)
            {
                city.cityStats.Inventory.Add(new CityResource(CityResource.Type.Gold, city.cityGateToll));
                visitor.PaidEntryToll = true;
            }
        }
    }

    public override void Despawn()
    {
        base.city.cityGates.Remove(this);
        base.Despawn();
    }

    public override Task GetWorkTask(Citizen citizen)
    {
        throw new System.NotImplementedException();
    }
}
