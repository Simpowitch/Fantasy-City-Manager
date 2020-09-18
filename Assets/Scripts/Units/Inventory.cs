using System.Collections.Generic;

public class Inventory
{
    public List<CityResource> resources { get; private set; } = new List<CityResource>();

    public void Add(CityResource resourceToAdd)
    {
        foreach (var existingResource in resources)
        {
            if (resourceToAdd.type == existingResource.type)
            {
                existingResource.Value += resourceToAdd.Value;
                return;
            }
        }
        resources.Add(resourceToAdd);
    }

    public bool TryToRemove(CityResource resourceToRemove)
    {
        foreach (var existingResource in resources)
        {
            if (resourceToRemove.type == existingResource.type)
            {
                if (existingResource.Value >= resourceToRemove.Value)
                {
                    return false;
                }
                else
                {
                    existingResource.Value += resourceToRemove.Value;
                    return true;
                }
            }
        }
        return false;
    }
}
