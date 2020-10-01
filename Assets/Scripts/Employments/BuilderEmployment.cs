using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuilderEmployment : Employment
{
    const float BUILDTIME = 2f;
    public List<Structure> unfinishedStructures;
    List<Structure> AvailableForConstruction
    {
        get
        {
            List<Structure> available = new List<Structure>();
            available.PopulateListWithMatchingConditions(unfinishedStructures, (structure) => structure.constructionArea.CanBeWorkedOn); //Where the last tick is not being worked on by anyone
            return available;
        }
    }

    Structure targetStructure;

    public override bool ShiftActive => AvailableForConstruction.Count > 0;

    public override Task GetWorkTask(Citizen citizen)
    {
        if (AvailableForConstruction.Count > 0) //At least 1 unfinished structure with atleast 1 tick left
        {
            if (targetStructure == null)
                targetStructure = Utility.GetClosest(unfinishedStructures, citizen);
            targetStructure.constructionArea.OccupyTick();
            ActionTimer onTaskEndTimer = new ActionTimer(BUILDTIME, WorkAction, false);
            Vector3 pos = targetStructure.GetRandomLocation();
            Vector3 dir = pos - citizen.transform.position;
            return new Task("Constructing", ThoughtFileReader.GetText(citizen.UnitPersonality, "constructing"), onTaskEndTimer, pos, () => citizen.UnitAnimator.PlayBuildAnimation(dir));
        }
        else //No unfinished structures
        {
            Debug.LogError("Tried to get task when unfinished structures are less than 1");
            return new Task("Practicing hammering nails", ThoughtFileReader.GetText(citizen.UnitPersonality, ""), new ActionTimer(3, null, false), citizen.transform.position);
        }
    }

    private void WorkAction()
    {
        targetStructure.constructionArea.AddConstructionTick();
        targetStructure.constructionProgressBar.SetNewValues(targetStructure.constructionArea.GetConstructionTickNormalized);
        if (targetStructure.constructionArea.IsFinished)
            targetStructure = null;
    }
}
