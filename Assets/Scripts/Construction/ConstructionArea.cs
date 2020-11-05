using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructionArea
{
    private Action onConstructionComplete;
    [SerializeField] float defaultTimeToComplete = 2f; public float DefaultTimeToComplete => defaultTimeToComplete; //To be used by AI primarily

    public bool occupied;
    public List<ObjectTile> ObjectTiles { get; private set; }

    public void Setup(Action onConstructionComplete, List<ObjectTile> objectTiles)
    {
        this.onConstructionComplete = onConstructionComplete;
        ObjectTiles = objectTiles;
    }

    public void CompleteConstruction() => onConstructionComplete?.Invoke();

    // Player Interaction with the constructionarea - Returns true if possible
    public bool PlayerInteraction(PlayerCharacter playerCharacter, PlayerInput playerInput, PlayerTaskSystem playerTaskSystem, Vector3 position)
    {
        if (occupied)
            return false;

        playerCharacter.MoveTo(position, UnitAnimator.ActionAnimation.Build, true);

        occupied = true;

        //Start Task
        playerTaskSystem.StartTask(() =>
        {
            CompleteConstruction();
            playerCharacter.UnitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.Idle);
            playerInput.inputEnabled = true;
            playerCharacter.SetColliderState(true);
        });

        return true;
    }
}
