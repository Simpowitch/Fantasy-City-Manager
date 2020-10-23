
public class SecurityDistrict : District
{
    public enum Subtype
    {
        TrainingGrounds,
        Guardpost,
        Stronghold
    }
    Subtype subtype;

    public override int DistrictCategoryIndex => (int)DistrictCategory.Security;
    public override int DistrictSubType => (int)subtype;

    public SecurityDistrict(int index, HexCell hexCell) : base("New Security District", hexCell)
    {
        subtype = (Subtype)index;
    }
    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
