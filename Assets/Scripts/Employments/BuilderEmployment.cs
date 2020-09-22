using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuilderEmployment : Employment
{
    const float BUILDTIME = 0.5f;
    public List<Structure> unfinishedStructures;

    Structure targetStructure;

    public override bool ShiftActive => unfinishedStructures.Count > 0;

    public override Task GetWorkTask(Citizen citizen)
    {
        if (unfinishedStructures.Count > 0) //At least 1 unfinished structure
        {
            if (targetStructure == null)
                targetStructure = Utility.GetClosest(unfinishedStructures, citizen);

            ActionTimer onTaskEndTimer = new ActionTimer(BUILDTIME, WorkAction, false);
            return new Task("Constructing", ThoughtFileReader.GetText(citizen.UnitPersonality, "constructing"), onTaskEndTimer, targetStructure.GetRandomLocation());
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
