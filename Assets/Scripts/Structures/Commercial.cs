
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

    protected override void HourChanged(int newHour)
    {
        base.HourChanged(newHour);
        
    }
}