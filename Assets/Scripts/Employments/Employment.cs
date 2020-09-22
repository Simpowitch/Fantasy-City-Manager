
[System.Serializable]
public class Employment
{
    public Workplace workplace;
    public Citizen employee;
    public string employmentName = "worker";

    public void Employ(Citizen citizen) => employee = citizen;
    public void QuitJob() => employee = null;

    public bool PositionFilled { get => employee != null; }
    public virtual Task GetWorkTask(Citizen citizen) => workplace.GetWorkTask(citizen);
    public virtual bool ShiftActive => workplace.ShiftActive;
}
