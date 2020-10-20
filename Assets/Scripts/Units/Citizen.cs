using UnityEngine;
using System.Collections.Generic;

public class Citizen : Unit, IViewable
{
    Residence home;
    Employment employment;

    [SerializeField] BodypartSpriteGroup[] unemployedClothingGroups = null;

    #region Needs
    public Need Hunger { private set; get; }
    public Need Energy { private set; get; }
    public Need Recreation { private set; get; }
    public Need Social { private set; get; }
    #endregion

    private void OnDestroy()
    {
        if (employment != null)
            employment.QuitJob();
        City.cityStats.Population--;
    }

    public void Setup(City city)
    {
        base.BaseSetup(city);
        Body.SetBodypartSpriteGroup(Body.Bodypart.Body, Utility.ReturnRandom(unemployedClothingGroups));

        LookForHome();
        LookForEmployment();
        City.cityStats.Population++;

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

    protected override void NewHour(int hour)
    {
        base.NewHour(hour);
        if (employment == null)
        {
            Body.SetBodypartSpriteGroup(Body.Bodypart.Body, Utility.ReturnRandom(unemployedClothingGroups));
            LookForEmployment();
        }
        if (!home)
            LookForHome();
    }

    protected override void InfoChanged() => InfoChangeHandler?.Invoke(this);

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
            Body.SetBodypartSpriteGroup(Body.Bodypart.Body, employment.WorkClothing);
        }
    }
    public void QuitJob()
    {
        Body.SetBodypartSpriteGroup(Body.Bodypart.Body, Utility.ReturnRandom(unemployedClothingGroups));
        employment.QuitJob();
        employment = null;
        LookForEmployment();
    }

    public string GetProfession() => employment != null ? employment.employmentName : "Unemployed";

    #region Tasks and Needs
    protected override void FindNewTask()
    {
        if (employment != null && employment.ShiftActive)
            CurrentTask = employment.GetWorkTask(this);
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

        CurrentTask = GetTask(chosenNeed);
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
        if (home)
            return home.CreateSatisfyNeedTask(this, Energy);
        else
            return null;
    }

    private Task CreateHungerTask()
    {
        if (City.HungerProviders.Count > 0)
        {
            INeedProvider source = Utility.ReturnRandom(City.HungerProviders);
            return source.CreateSatisfyNeedTask(this, Hunger);
        }
        else
            return null;
    }
    private Task CreateRecreationTask()
    {
        if (City.RecreationProviders.Count > 0)
        {
            INeedProvider source = Utility.ReturnRandom(City.RecreationProviders);
            return source.CreateSatisfyNeedTask(this, Recreation);
        }
        else
            return null;
    }
    private Task CreateSocialTask()
    {
        if (City.SocialProviders.Count > 0)
        {
            INeedProvider source = Utility.ReturnRandom(City.SocialProviders);
            return source.CreateSatisfyNeedTask(this, Social);
        }
        else
            return null;
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

    public string ActionDescription { get => CurrentTask.Description; }
    public string Name
    {
        get => UnitName;
        set
        {
            UnitName = value;
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
    public string GetSpeciality() => GetProfession();
    public float GetPrimaryStatValue() => Happiness;
    public string GetPrimaryStatName() => "Happiness";
    #endregion
}
