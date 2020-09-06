using UnityEngine;

public class Visitor : Unit
{
    [SerializeField] [Range(0, 100)] int leaveRandomizerPercentage = 20;
    Structure inn;
    public bool PaidEntryToll { get; set; } = false;

    private void OnDestroy()
    {
        City.cityStats.Visitors--;
    }

    public void Setup(City city)
    {
        base.BaseSetup(city);
        this.inn = Utility.ReturnRandom(City.taverns);
        ChangeState(new VisitCommercial(this));
        City.cityStats.Visitors++;

        //Needs initializing
        Health = new Need("Health", 0, 0, 0.25f, 0.75f, 1, new MoodBuff("Dying", -100), new MoodBuff("Badly hurt", -50), new MoodBuff("Hurt", -10), new MoodBuff("Healthy", 25));
        Food = new Need("Food", 0.05f, 0, 0.25f, 0.75f, 1f, new MoodBuff("Starving", -100), new MoodBuff("Hungry", -50), null, new MoodBuff("Ate recently", 25));
        Employment = new Need("Employment", 0.01f, 0, 0.25f, 0.75f, 0.74f, new MoodBuff("No livelyhood", -50), new MoodBuff("Lacking purpose", -25), null, new MoodBuff("Honest day of work", 25));
        Recreation = new Need("Recreation", 0.02f, 0, 0.25f, 0.75f, 0.5f, new MoodBuff("Depressed", -50), new MoodBuff("Bored", -20), null, new MoodBuff("Had fun", 15));
        Faith = new Need("Faith", 0.01f, 0, 0.25f, 0.75f, 0.74f, new MoodBuff("Godforsaken", -10), new MoodBuff("Not in touch with faith", -5), null, new MoodBuff("Blessed", 10));
        Hygiene = new Need("Hygiene", 0.01f, 0, 0.25f, 0.75f, 1f, new MoodBuff("Stinking", -10), new MoodBuff("Smelly", -5), null, new MoodBuff("Clean", 10));

        Health.OnNeedValuesChanged += CalculateMoodBuffs;
        Food.OnNeedValuesChanged += CalculateMoodBuffs;
        Employment.OnNeedValuesChanged += CalculateMoodBuffs;
        Recreation.OnNeedValuesChanged += CalculateMoodBuffs;
        Faith.OnNeedValuesChanged += CalculateMoodBuffs;
        Hygiene.OnNeedValuesChanged += CalculateMoodBuffs;
    }

    protected override void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay)
    {
        if (FSM is LeaveCity)
        {
            return;
        }
        switch (partOfDay)
        {
            case DayNightSystem.PartOfTheDay.Night:
            case DayNightSystem.PartOfTheDay.Evening:
                ChangeState(new GoToInn(this));
                break;
            case DayNightSystem.PartOfTheDay.Morning:
            case DayNightSystem.PartOfTheDay.Day:
                ChangeState(new VisitCommercial(this));
                break;
        }
    }


    protected override void NewNodeReached(PathNode newNode)
    {
        if (newNode == null)
        {
            if (FSM is LeaveCity)
            {
                FSM.ExitState();
                return;
            }
            if (FSM is VisitCommercial)
            {
                //Leave or visit another commercial building
                if (Utility.RandomizeBool(leaveRandomizerPercentage))
                {
                    ChangeState(new LeaveCity(this));
                }
                else
                {
                    ChangeState(new VisitCommercial(this));
                }
            }
        }
        base.NewNodeReached(newNode);
    }

    protected void SendThought(string thought) => StartCoroutine(messageDisplay.ShowMessage(3, thought, MessageDisplay.MessageType.Chatbubble));

    public override string GetProfession() => "Visitor";

    #region FSM
    public abstract class VisitorState : State
    {
        protected Visitor visitor;

        public VisitorState(Visitor visitor) : base(visitor)
        {
            this.visitor = visitor;
        }
    }

    public class GoToInn : VisitorState
    {
        static string[] ENTERSTATEPHRASES = new string[] { "Need to look for a good place to stay the night", "Anything open?", "Need to find a vacant bed", "It's not safe to travel now", "I shall stay in this town overnight" };

        public GoToInn(Visitor visitor) : base(visitor) { }

        public override void EnterState()
        {
            visitor.Seeker.FindPathTo(visitor.inn.CenterPosition);
            if (RNG.PercentageIntTry(10))
            {
                visitor.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
            }
            base.EnterState();
        }

        public override void ExitState() { }
    }

    public class VisitCommercial : VisitorState
    {
        public VisitCommercial(Visitor visitor) : base(visitor) { }

        public override void EnterState()
        {
            Structure chosenTarget = Utility.ReturnRandom(visitor.City.commercialBuidlings);
            visitor.Seeker.FindPathTo(chosenTarget.CenterPosition);
            if (RNG.PercentageIntTry(10))
            {
                visitor.SendThought($"I need to visit the {chosenTarget}");
            }
            base.EnterState();
        }

        public override void ExitState() { }
    }

    public class LeaveCity : VisitorState
    {
        static string[] ENTERSTATEPHRASES = new string[] { "I think I have seen all this city has to offer", "This town was welcoming", "I'll soon visit again", "I will not be coming back here" };

        public LeaveCity(Visitor visitor) : base(visitor) { }

        public override void EnterState()
        {
            //Go to city edge
            Transform chosenTarget = Utility.ReturnRandom(visitor.City.cityEntrances);
            visitor.Seeker.FindPathTo(chosenTarget.position);
            if (RNG.PercentageIntTry(10))
            {
                visitor.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
            }
            base.EnterState();
        }

        public override void ExitState()
        {
            //Destroy
            GameObject.Destroy(unit.gameObject);
        }
    }
    #endregion
}
