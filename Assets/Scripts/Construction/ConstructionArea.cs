using System;
using UnityEngine;

[System.Serializable]
public class ConstructionArea
{
    [SerializeField] int constructionTick = 0;
    [SerializeField] int constructionTickMax = 10;
    private Action onConstructionComplete;
    int ticksOccupied;
    public bool IsFinished { get => constructionTick >= constructionTickMax; }
    public bool CanBeWorkedOn { get => constructionTick + ticksOccupied < constructionTickMax; }
    public void Setup(Action onConstructionComplete)
    {
        this.onConstructionComplete = onConstructionComplete;
    }


    public void OccupyTick() => ticksOccupied++;

    public void AddConstructionTick()
    {
        ticksOccupied--;
        constructionTick++;
        if (IsFinished)
        {
            onConstructionComplete();
        }
    }

    public float GetConstructionTickNormalized => constructionTick * 1f / constructionTickMax;
}
