﻿using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    protected ResourceObjectNetwork network;
    public ObjectTile ObjectTile { get; private set; }

    [SerializeField] CityResource yieldOnHarvest = null;
    [SerializeField] SpriteRenderer markRenderer = null;
    public CityResource.Type Type { get => yieldOnHarvest.type; }

    [HideInInspector] public Citizen workOccupiedBy;
    public bool CanBeHarvested { get; protected set; } = true;
    public bool MarkedForHarvest { get; private set; }
    public enum HarvestMarkMode { Automatic, Manual}
    [SerializeField] HarvestMarkMode harvestMode = HarvestMarkMode.Manual;
    public HarvestMarkMode HarvestMode { get => harvestMode; }

    //Called when the object is spawned in
    public virtual void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        this.network = network;
        this.ObjectTile = objectTile;
        this.ObjectTile.ResourceObject = this;
    }

    public CityResource Harvest()
    {
        Despawn();
        return yieldOnHarvest;
    }

    public void MarkForHarvest(bool state)
    {
        MarkedForHarvest = state;
        markRenderer.enabled = state;
    }

    //Called when harvested 
    protected virtual void Despawn()
    {
        network.RemoveStatic(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject, 0.01f);
    }
}
