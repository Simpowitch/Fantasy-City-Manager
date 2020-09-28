using UnityEngine;

public class GrowingResource : ResourceObject
{
    [SerializeField] bool markForHarvestWhenGrown = false;
    [SerializeField] float timeToHarvestable = 60;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite grownStage = null;

    public bool CanBeHarvested { get; private set; } = false;

    private void Start()
    {
        ActionTimer growTimer = new ActionTimer(timeToHarvestable, FullyGrown, true);
    }

    void FullyGrown()
    {
        spriteRenderer.sprite = grownStage;
        if (markForHarvestWhenGrown)
            MarkForHarvest(true);
        CanBeHarvested = true;
    }

    protected override void Despawn()
    {
        network.RemoveGrowable(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject, 0.01f);
    }
}