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
                if (existingResource.Value < resourceToRemove.Value)
                {
                    return false;
                }
                else
                {
                    existingResource.Value -= resourceToRemove.Value;
                    if (existingResource.Value <= 0)
                        resources.Remove(existingResource);
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveAllOfType(CityResource.Type type, out CityResource resourcesRemoved)
    {
        resourcesRemoved = new CityResource(type, 0);
        foreach (var resource in resources)
        {
            if (resource.type == type)
            {
                resourcesRemoved.Value += resource.Value;
                TryToRemove(resource);
            }
        }
    }

    public bool HasResourceType(CityResource.Type type)
    {
        foreach (var resource in resources)
        {
            if (resource.type == type)
                return true;
        }
        return false;
    }
}
