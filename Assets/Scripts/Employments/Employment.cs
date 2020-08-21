using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Employment
{
    public Citizen employee;
    public string employmentName = "worker";

    public void Employ(Citizen citizen) => employee = citizen;
    public void QuitJob() => employee = null;

    public bool PositionFilled { get => employee != null; }

    public List<Vector3> workingPositions;

    public virtual WorkState GetWorkState()
    {
        return new WorkState(employee, this);
    }

    public class WorkState : Citizen.CitizenState
    {
        static string[] ENTERSTATEPHRASES = new string[] { "Time to do my job", "I love my work", "This is gonna be a good day", "It's a tough life", "Atleast I can make a living", "Working outside in the fresh air would feel so much better",
        "Hard work pays best", "It's a shitty job, but it pays", "I feel tired already"};

        protected Employment employment;
        public WorkState(Citizen citizen, Employment employment) : base(citizen)
        {
            this.employment = employment;
        }

        public override void EnterState()
        {
            Vector3 targetPosition = citizen.transform.position;
            if (employment.workingPositions != null && employment.workingPositions.Count > 0)
                targetPosition = Utility.ReturnRandom(employment.workingPositions);

            citizen.Seeker.FindPathTo(targetPosition);
            if (RNG.PercentageIntTry(10))
            {
                citizen.SendThought(Utility.ReturnRandom(ENTERSTATEPHRASES));
            }
            base.EnterState();
        }
    }
}
