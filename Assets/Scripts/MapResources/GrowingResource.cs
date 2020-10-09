using UnityEngine;

public class GrowingResource : ResourceObject
{
    [SerializeField] float timeToHarvestable = 60;
    [SerializeField] GameObject[] saplingStage = null;
    [SerializeField] GameObject[] grownStage = null;
    ActionTimer growTimer;
    [SerializeField] Animator animator = null;
    string growAnimationName = "Grow";
    string harvestCompletedAnimationName = "HarvestEnd";
    string harvestingAnimationName = "Harvesting";

    public override void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        ChangeRendererStage(false);
        CanBeHarvested = false;
        growTimer = new ActionTimer(timeToHarvestable, FullyGrown, true);
        base.Spawned(network, objectTile);
        InvokeRepeating("InfoChanged", 0, 0.1f);
    }

    private void InfoChanged() => InfoChangeHandler?.Invoke(this);

    public override void StartHarvesting()
    {
        if (animator) 
            animator.SetTrigger(harvestingAnimationName);
    }

    public override CityResource Harvest()
    {
        if (animator)
            animator.SetTrigger(harvestCompletedAnimationName);
        return base.Harvest();
    }

    void FullyGrown()
    {
        ChangeRendererStage(true);
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
        GameObject.Destroy(this.gameObject, 2f);
    }

    public override float GetPrimaryStatValue() => growTimer.Progress;

    public override string GetPrimaryStatName() => "Growth";

    private void ChangeRendererStage(bool nextStage)
    {
        foreach (var gameObject in saplingStage)
        {
            gameObject.SetActive(!nextStage);
        }
        foreach (var gameObject in grownStage)
        {
            gameObject.SetActive(nextStage);
        }
        if (nextStage)
        {
            if (animator)
                animator.SetTrigger(growAnimationName);
        }
    }
}