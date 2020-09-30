using UnityEngine;
using System.Collections.Generic;

public class Visitor : Unit, IViewable
{
    [SerializeField] [Range(0, 100)] int leaveRandomizerPercentage = 20;
    Structure inn;
    public bool PaidEntryToll { get; set; } = false;

    #region Needs
    public Need Hunger { private set; get; }
    public Need Energy { private set; get; }
    public Need Recreation { private set; get; }
    public Need Social { private set; get; }

    #endregion

    private void OnDestroy()
    {
        City.cityStats.Visitors--;
    }

    public void Setup(City city)
    {
        base.BaseSetup(city);
        this.inn = Utility.ReturnRandom(City.taverns);
        City.cityStats.Visitors++;

        //Needs
        Energy = new Need(Need.NeedType.Energy, 0.05f, 1);
        Energy.OnNeedValuesChanged += CalculateHappiness;
        Hunger = new Need(Need.NeedType.Hunger, 0.05f, 1);
        Hunger.OnNeedValuesChanged += CalculateHappiness;
        Recreation = new Need(Need.NeedType.Recreation, 0.05f, 1);
        Recreation.OnNeedValuesChanged += CalculateHappiness;
        Social = new Need(Need.NeedType.Social, 0.05f, 1);
        Social.OnNeedValuesChanged += CalculateHappiness;
    }

    protected override void InfoChanged() => InfoChangeHandler?.Invoke(this);

    #region Tasks and Needs
    protected override void FindNewTask()
    {
        if (Utility.RandomizeBool(leaveRandomizerPercentage))
            currentTask = City.CreateLeaveCityTask(this);
        else
            FindNeedFullfillTask();
        base.FindNewTask();
    }

    private void FindNeedFullfillTask()
    {
        List<Need> lowNeeds = new List<Need>();
        List<Need> mediumNeeds = new List<Need>();

        foreach (Need need in GetNeeds())
        {
            if (need.State == Need.NeedState.Low)
                lowNeeds.Add(need);
            else if (need.State == Need.NeedState.Medium)
                mediumNeeds.Add(need);
        }

        Need chosenNeed;
        if (lowNeeds.Count > 0)
            chosenNeed = Utility.ReturnRandom(lowNeeds);
        else if (mediumNeeds.Count > 0)
            chosenNeed = Utility.ReturnRandom(mediumNeeds);
        else
            chosenNeed = Utility.ReturnRandom(GetNeeds());

        currentTask = GetTask(chosenNeed);
    }

    private Task GetTask(Need need)
    {
        switch (need.Type)
        {
            case Need.NeedType.Energy:
                return CreateEnergyTask();
            case Need.NeedType.Hunger:
                return CreateHungerTask();
            case Need.NeedType.Recreation:
                return CreateRecreationTask();
            case Need.NeedType.Social:
                return CreateSocialTask();
            default:
                Debug.LogError("Need not defined");
                return null;
        }
    }

    private Task CreateEnergyTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.EnergyProviders);
        return source.CreateSatisfyNeedTask(this, Energy);
    }
    private Task CreateHungerTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.HungerProviders);
        return source.CreateSatisfyNeedTask(this, Hunger);
    }
    private Task CreateRecreationTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.RecreationProviders);
        return source.CreateSatisfyNeedTask(this, Recreation);
    }
    private Task CreateSocialTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.SocialProviders);
        return source.CreateSatisfyNeedTask(this, Social);
    }

    private void CalculateHappiness()
    {
        float average = 0;
        average += Energy.CurrentValue;
        average += Hunger.CurrentValue;
        average += Recreation.CurrentValue;
        average += Social.CurrentValue;
        average /= 4;
        Happiness = average;
        InfoChangeHandler?.Invoke(this);
    }
    #endregion

    #region Viewable interface

    public InfoChangeHandler InfoChangeHandler { get; set; }
    public string ActionDescription { get => currentTask.Description; }

    public string Name
    {
        get => unitName;
        set
        {
            unitName = value;
            InfoChanged();
        }
    }

    public Need[] GetNeeds()
    {
        Need[] needs = new Need[4];
        needs[0] = Energy;
        needs[1] = Hunger;
        needs[2] = Recreation;
        needs[3] = Social;
        return needs;
    }
    public string GetSpeciality() => "Visitor";
    public float GetPrimaryStatValue() => Happiness;
    public string GetPrimaryStatName() => "Happiness";
    #endregion
}
