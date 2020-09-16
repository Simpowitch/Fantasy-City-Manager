using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuilderEmployment : Employment
{
    const float BUILDTIME = 0.5f;
    public List<Structure> unfinishedStructures;

    Structure targetStructure;



    public Task CreateTask(Unit unit)
    {
        Task newTask = new Task();
        if (unfinishedStructures.Count > 0) //At least 1 unfinished structure
        {
            if (targetStructure == null)
                targetStructure = Utility.GetClosest(unfinishedStructures, unit);

            newTask.CreateAndAddSubTask(unit, "Hammering", targetStructure.GetRandomLocation(), BUILDTIME, WorkAction);
        }
        else //No unfinished structures
        {
            newTask.CreateAndAddSubTask(unit, "Practicing crafting a wooden horse", unit.transform.position, 1f, null);
        }
        return newTask;
    }

    private void WorkAction()
    {
        targetStructure.constructionArea.AddConstructionTick();
        targetStructure.constructionProgressBar.SetNewValues(targetStructure.constructionArea.GetConstructionTickNormalized);
        if (targetStructure.constructionArea.IsFinished)
            targetStructure = null;
    }

    //public override WorkState GetWorkState()
    //{
    //    return new BuilderWorkState(employee, this);
    //}

    //public class BuilderWorkState : WorkState
    //{
    //    static string[] ENTERSTATEPHRASES = new string[] { "Time to get some structures built", "Nothing beats creating new places for people to be in", "I hope I won't hammer my hand again" };
    //    BuilderEmployment builderEmployment;
    //    Structure targetStructure;
    //    ActionTimer buildTimer;
    //    public BuilderWorkState(Citizen citizen, BuilderEmployment employment) : base(citizen, employment) { this.builderEmployment = employment; }

    //    public override void EnterState()
    //    {
    //        SetWorkerTargets();

    //        if (RNG.PercentageIntTry(10))
    //        {
    //            citizen.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
    //        }
    //        base.EnterState();
    //    }


    //    public override void DuringState()
    //    {
    //        if (!targetStructure)
    //            SetWorkerTargets();
    //        if (!citizen.Seeker.HasPath)
    //        {
    //            if (buildTimer == null || buildTimer.IsFinished)
    //            {
    //                buildTimer = new ActionTimer(BUILDTIME, () => WorkAction());
    //            }
    //        }
    //        base.DuringState();
    //    }

    //    private void WorkAction()
    //    {
    //        targetStructure.constructionArea.AddConstructionTick();
    //        targetStructure.constructionProgressBar.SetNewValues(targetStructure.constructionArea.GetConstructionTickNormalized);
    //        if (targetStructure.constructionArea.IsFinished)
    //            SetWorkerTargets();
    //    }

    //    private void SetWorkerTargets()
    //    {
    //        Vector3 targetPosition = citizen.transform.position;
    //        if (builderEmployment.unfinishedStructures != null && builderEmployment.unfinishedStructures.Count > 0)
    //        {
    //            targetStructure = Utility.ReturnRandom(builderEmployment.unfinishedStructures);
    //            if (targetStructure != null)
    //                targetPosition = Utility.ReturnRandom(targetStructure.ObjectTiles).CenteredWorldPosition;
    //        }
    //        citizen.Seeker.FindPathTo(targetPosition);
    //    }
    //}
}
