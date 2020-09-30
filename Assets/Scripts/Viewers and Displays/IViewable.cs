
public delegate void InfoChangeHandler(IViewable viewable);
public interface IViewable
{
    InfoChangeHandler InfoChangeHandler { get; set; }
    string Name { get; set; }
    string ActionDescription { get; }
    string GetSpeciality();
    string GetPrimaryStatName();
    float GetPrimaryStatValue();
    Need[] GetNeeds();
}
