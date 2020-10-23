
public class CultureDistrict : District
{
    public enum SubType
    {
        Entertainment,
        Religious,
        Landmark,
        Park
    }
    SubType subtype;

    public override int DistrictCategoryIndex => (int)DistrictCategory.Culture;
    public override int DistrictSubType => (int)subtype;

    public CultureDistrict(int index, HexCell hexCell) : base("New Cultural District", hexCell)
    {
        subtype = (SubType)index;
    }

    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
