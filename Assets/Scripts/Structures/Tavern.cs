
public class Tavern : Commercial
{
    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.taverns.Add(this);
    }

    public override void Despawn()
    {
        base.city.taverns.Remove(this);
        base.Despawn();
    }
}
