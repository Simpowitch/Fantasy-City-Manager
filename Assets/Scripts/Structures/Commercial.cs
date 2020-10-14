using UnityEngine;
using System.Collections.Generic;

public class Commercial : Workplace, INeedProvider
{
    [Header("Commercial")]
    [SerializeField] CityResource consumedOfPatreon = null;
    [SerializeField] List<Need.NeedType> providedNeeds = null;

    [Header("Patreon Task setup")]
    [SerializeField] protected string patreonTaskThoughtHeader = "";
    [SerializeField] protected string patreonTaskDescription = "";
    [SerializeField] Transform patreonTaskTileParent = null;
    protected PatreonTaskTile[] PatreonTaskTiles { get; private set; } = null;

    public List<Need.NeedType> NeedTypes => providedNeeds;

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.commercialBuidlings.Add(this);

        foreach (var needType in NeedTypes)
        {
            switch (needType)
            {
                case Need.NeedType.Energy:
                    city.EnergyProviders.Add(this);
                    break;
                case Need.NeedType.Hunger:
                    city.HungerProviders.Add(this);
                    break;
                case Need.NeedType.Recreation:
                    city.RecreationProviders.Add(this);
                    break;
                case Need.NeedType.Social:
                    city.SocialProviders.Add(this);
                    break;
            }
        }

        //Set up patreon task tiles
        PatreonTaskTiles = new PatreonTaskTile[patreonTaskTileParent.childCount];
        for (int i = 0; i < PatreonTaskTiles.Length; i++)
        {
            PatreonTaskTiles[i] = new PatreonTaskTile(city.ObjectGrid.GetGridObject(patreonTaskTileParent.GetChild(i).position), patreonTaskTileParent.GetChild(i));
        }
    }

    public override void Despawn()
    {
        base.city.commercialBuidlings.Remove(this);
        base.Despawn();
    }

    public Task CreateSatisfyNeedTask(Unit unit, Need needToSatisfy)
    {
        if (!NeedTypes.Contains(needToSatisfy.Type))
        {
            Debug.LogError("Need asked for cannot be provided by this building");
            return null;
        }

        PatreonTaskTile freeTile = Utility.ReturnRandomElementWithCondition(PatreonTaskTiles, (tile) => !tile.Occupied);
        if (freeTile != null)
        {
            freeTile.Occupied = true;

            ActionTimer onTaskEnd = new ActionTimer(3f, () =>
            {
                needToSatisfy.Satisfy();
                city.cityStats.Inventory.TryToRemove(consumedOfPatreon);
                freeTile.Occupied = false;
            }, false);
            return new Task(patreonTaskDescription, ThoughtFileReader.GetText(unit.UnitPersonality, patreonTaskThoughtHeader), onTaskEnd, freeTile.ObjectTile.CenteredWorldPosition, () => unit.UnitAnimator.PlayActionAnimation(freeTile.ForwardDirection, UnitAnimator.ActionAnimation.Drink));
        }
        else
            return Task.CreateIdleTask("Waiting", $"The {transform.name} is too crowded", unit.transform.position, unit);
    }


    protected class PatreonTaskTile
    {
        public ObjectTile ObjectTile { get; private set; }
        public bool Occupied { get; set; }

        public Direction2D ForwardDirection { get; private set; }

        public PatreonTaskTile(ObjectTile objectTile, Transform transform)
        {
            ObjectTile = objectTile;
            ForwardDirection = Utility.GetDirection(transform);
        }
    }
}