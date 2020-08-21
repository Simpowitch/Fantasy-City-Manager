using System;
using UnityEngine;

[System.Serializable]
public class ConstructionArea
{
    [SerializeField] int constructionTick = 0;
    [SerializeField] int constructionTickMax = 10;
    private Action onConstructionComplete;
    public bool IsFinished { get => constructionTick >= constructionTickMax; }
    public void Setup(Action onConstructionComplete)
    {
        this.onConstructionComplete = onConstructionComplete;
    }

    public void AddConstructionTick()
    {
        constructionTick++;
        if (IsFinished)
        {
            onConstructionComplete();
        }
    }

    public float GetConstructionTickNormalized => constructionTick * 1f / constructionTickMax;
}
