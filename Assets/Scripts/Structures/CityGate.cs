
public class CityGate : Workplace
{
    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.cityGates.Add(this);
    }
    public override void InteractedWith(Unit unitVisiting)
    {
        base.InteractedWith(unitVisiting);
        if (unitVisiting is Visitor)
        {
            city.cityStats.Gold.Value += city.cityGateToll;
        }
    }

    public override void Despawn()
    {
        base.city.cityGates.Remove(this);
        base.Despawn();
    }
}
