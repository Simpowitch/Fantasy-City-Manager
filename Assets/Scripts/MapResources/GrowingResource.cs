using UnityEngine;

public class GrowingResource : ResourceObject
{
    [SerializeField] float timeToHarvestable = 60;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite grownStage = null;
    public override void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        CanBeHarvested = false;
        ActionTimer growTimer = new ActionTimer(timeToHarvestable, FullyGrown, true);
        base.Spawned(network, objectTile);
    }

    void FullyGrown()
    {
        spriteRenderer.sprite = grownStage;
        if (HarvestMode == HarvestMarkMode.Automatic)
            MarkForHarvest(true);
        CanBeHarvested = true;
    }

    protected override void Despawn()
    {
        if (network)
            network.RemoveGrowable(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject, 0.01f);
    }
}