
public class Commercial : Workplace
{
    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.commercialBuidlings.Add(this);
    }

    public override void Despawn()
    {
        base.city.commercialBuidlings.Remove(this);
        base.Despawn();
    }
}
