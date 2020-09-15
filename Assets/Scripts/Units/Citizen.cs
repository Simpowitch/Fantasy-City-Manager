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
        //ChangeState(new VisitCommercial(this));
        City.cityStats.Population++;
    }

    protected override void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay)
    {
        //switch (partOfDay)
        //{
        //    case DayNightSystem.PartOfTheDay.Night:
        //        ChangeState(new GoHome(this));
        //        break;
        //    case DayNightSystem.PartOfTheDay.Morning:
        //    case DayNightSystem.PartOfTheDay.Evening:
        //        ChangeState(new VisitCommercial(this));
        //        break;
        //    case DayNightSystem.PartOfTheDay.Day:
        //        if (employment != null)
        //            ChangeState(employment.GetWorkState());
        //        else
        //            ChangeState(new VisitCommercial(this));
        //        break;
        //}
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

    public void SendThought(string thought) => StartCoroutine(messageDisplay.ShowMessage(3, thought, MessageDisplay.MessageType.Chatbubble));

    public override string GetProfession() => employment != null ? employment.employmentName : "Unemployed";

    //#region FSM
    //public abstract class CitizenState : State
    //{
    //    protected Citizen citizen;

    //    public CitizenState(Citizen citizen) : base(citizen)
    //    {
    //        this.citizen = citizen;
    //    }
    //}

    //public class WorkState : CitizenState
    //{
    //    public WorkState(Citizen citizen) : base(citizen) { }

    //}

    ////public class GoHome : CitizenState
    ////{
    ////    static string[] ENTERSTATEPHRASES = new string[] {"I'm tired", "Time to hit the sack", "I hope the night won't be too cold", "Another day ends", "I pray for a safe night!",
    ////        "Hopefully it will be an uneventful night", "The star-filled sky is beautiful", "The moon shines upon us, gives us guidance", "The days seem to be shorter and shorter",
    ////        "I should look for some nightly company", "Time to join a warm bed", "My house is not too far from here, but I should head home", "The night is not safe out here"};
    ////    static string[] GREETINGS = new string[] { "Goodnight", "Night", "Evening", "See you tomorrow", "Sleep tight" };


    ////    public GoHome(Citizen citizen) : base(citizen) { }

    ////    public override void EnterState()
    ////    {
    ////        if (RNG.PercentageIntTry(10))
    ////        {
    ////            citizen.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
    ////        }
    ////        citizen.Seeker.FindPathTo(citizen.home.CenterPosition);
    ////        citizen.home.OnUnitVisiting += CheckHome;
    ////        base.EnterState();
    ////    }

    ////    public override void DuringState()
    ////    {
    ////        if (!citizen.messageDisplay.IsShowingMessage && RNG.FloatTry(0.001f))
    ////        {
    ////            if (citizen.unitDetector.SearchForUnits(citizen, 1).Count > 0)
    ////            {
    ////                citizen.SendThought(Utility.ReturnRandom(GREETINGS));
    ////            }
    ////        }
    ////        base.DuringState();
    ////    }

    ////    private void CheckHome(Unit arrivedUnit)
    ////    {
    ////        if (arrivedUnit == citizen)
    ////        {
    ////            citizen.home.OnUnitVisiting -= CheckHome;
    ////            citizen.ChangeState(new Sleep(citizen));
    ////        }
    ////    }
    ////}

    ////public class Sleep : CitizenState
    ////{
    ////    static string[] ENTERSTATEPHRASES = new string[] { "Home sweet home", "Honey, are you sleeping?", "Bedtime" };
    ////    static string[] DURINGPHRASES = new string[] {"My bed is cold", "I could use some company", "What was that...!?", "*SNORING*", "I should put some wood in the fire",
    ////        "...! NO, it was such a good dream", "Honey you are sleep-walking again", "I should build a better house", "The night is beautiful", "O' lord Cthulu, show your face once more",
    ////    "The night is so dark. It's full of terrors!", "I can't sleep"};

    ////    public Sleep(Citizen citizen) : base(citizen) { }

    ////    public override void EnterState()
    ////    {
    ////        if (RNG.PercentageIntTry(10))
    ////        {
    ////            citizen.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
    ////        }
    ////        base.EnterState();
    ////    }

    ////    public override void DuringState()
    ////    {
    ////        if (!citizen.messageDisplay.IsShowingMessage && RNG.FloatTry(0.0005f))
    ////        {
    ////            citizen.SendThought(Utility.ReturnRandom(DURINGPHRASES));
    ////        }
    ////        base.DuringState();
    ////    }
    ////}

    ////public class VisitCommercial : CitizenState
    ////{
    ////    public VisitCommercial(Citizen citizen) : base(citizen) { }

    ////    public override void EnterState()
    ////    {
    ////        Structure chosenTarget = Utility.ReturnRandom(citizen.City.commercialBuidlings);
    ////        citizen.Seeker.FindPathTo(chosenTarget.CenterPosition);
    ////        if (RNG.PercentageIntTry(10))
    ////        {
    ////            citizen.SendThought($"I need to visit the {chosenTarget}");
    ////        }

    ////        base.EnterState();
    ////    }
    ////}
    //#endregion
}
