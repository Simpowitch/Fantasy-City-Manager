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
        City.cityStats.Visitors++;
    }


    protected void SendThought(string thought) => StartCoroutine(messageDisplay.ShowMessage(3, thought, MessageDisplay.MessageType.Chatbubble));

    public override string GetProfession() => "Visitor";

    protected override void FindNewTask()
    {
        if (Utility.RandomizeBool(leaveRandomizerPercentage))
            currentTask = City.CreateLeaveCityTask(this);
        else
            FindNeedFullfillTask();
    }

    protected override Task CreateEnergyTask()
    {
        throw new System.NotImplementedException();
    }
}
