using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuilderEmployment : Employment
{
    ConstructionArea targetBlueprint;
    public override bool ShiftActive => AvailableForConstruction.Count > 0;

    public List<ConstructionArea> unfinishedConstructions;
    List<ConstructionArea> AvailableForConstruction
    {
        get
        {
            List<ConstructionArea> available = new List<ConstructionArea>();
            available.PopulateListWithMatchingConditions(unfinishedConstructions, (blueprint) => !blueprint.occupied); //Where the last tick is not being worked on by anyone
            return available;
        }
    }

    public override Task GetWorkTask(Citizen citizen)
    {
        ConstructionArea blueprint = Utility.ReturnRandom(AvailableForConstruction);

        if (blueprint != null)
        {
            targetBlueprint = blueprint;
            blueprint.occupied = true;
            ActionTimer onTaskEndTimer = new ActionTimer(blueprint.DefaultTimeToComplete, () => targetBlueprint.CompleteConstruction(), false);
            Vector3 pos = Utility.ReturnRandom(blueprint.ObjectTiles).CenteredWorldPosition;
            Vector3 dir = pos - citizen.transform.position;
            return new Task("Constructing", ThoughtFileReader.GetText(citizen.UnitPersonality, "constructing"), onTaskEndTimer, pos, UnitAnimator.ActionAnimation.Build);
        }
        else //No unfinished structures
        {
            Debug.LogError("Tried to get task when unfinished structures are less than 1");
            return new Task("Practicing hammering nails", ThoughtFileReader.GetText(citizen.UnitPersonality, ""), new ActionTimer(3, null, false), citizen.transform.position, UnitAnimator.ActionAnimation.Build);
        }
    }
}
