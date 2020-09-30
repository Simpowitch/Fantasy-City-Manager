using UnityEngine;

public class GrowingResource : ResourceObject
{
    [SerializeField] float timeToHarvestable = 60;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite grownStage = null;
    ActionTimer growTimer;
    public override void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        CanBeHarvested = false;
        growTimer = new ActionTimer(timeToHarvestable, FullyGrown, true);
        base.Spawned(network, objectTile);
        InvokeRepeating("InfoChanged", 0, 0.1f);
    }

    private void InfoChanged() => InfoChangeHandler?.Invoke(this);

    void FullyGrown()
    {
        spriteRenderer.sprite = grownStage;
        if (HarvestMode == HarvestMarkMode.Automatic)
            MarkForHarvest(true);
        CanBeHarvested = true;
        InfoChangeHandler?.Invoke(this);
    }

    protected override void Despawn()
    {
        if (network)
            network.RemoveGrowable(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject, 0.01f);
    }

    public override float GetPrimaryStatValue() => growTimer.Progress;

    public override string GetPrimaryStatName() => "Growth";
}