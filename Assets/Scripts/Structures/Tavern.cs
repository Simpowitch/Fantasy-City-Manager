using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : Commercial
{
    public override void InteractedWith(Unit unitVisiting)
    {
        base.InteractedWith(unitVisiting);
        if (unitVisiting.Food != null)
            unitVisiting.Food.FullfillNeed();
    }

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
