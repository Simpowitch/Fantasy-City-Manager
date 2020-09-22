using System.Collections.Generic;
using UnityEngine;

public class Citizen : Unit
{
    Residence home;
    Employment employment;

    private void OnDestroy()
    {
        if (employment != null)
            employment.QuitJob();
        City.cityStats.Population--;
    }

    public void Setup(City city)
    {
        base.BaseSetup(city);
        LookForHome();
        LookForEmployment();
        City.cityStats.Population++;
    }

    protected override void NewHour(int hour)
    {
        base.NewHour(hour);
        if (employment == null)
            LookForEmployment();
        if (!home)
            LookForHome();
    }

    private void LookForHome()
    {
        if (City.HasUnFilledResidences())
        {
            home = City.GetRandomFreeHome();
            home.MoveIn(this);
        }
    }
    public void LeaveHome()
    {
        home.MoveOut(this);
        home = null;
        LookForHome();
    }

    private void LookForEmployment()
    {
        if (City.UnFilledEmployments(out List<Employment> unfilledEmployments))
        {
            employment = Utility.ReturnRandom(unfilledEmployments);
            employment.Employ(this);
        }
    }
    public void QuitJob()
    {
        employment.QuitJob();
        employment = null;
        LookForEmployment();
    }

    public override string GetProfession() => employment != null ? employment.employmentName : "Unemployed";

    protected override void FindNewTask()
    {
        if (employment != null && employment.ShiftActive)
            currentTask = employment.GetWorkTask(this);
        else
            FindNeedFullfillTask();
        base.FindNewTask();
    }

    protected override Task CreateEnergyTask()
    {
        throw new System.NotImplementedException();
    }
}
