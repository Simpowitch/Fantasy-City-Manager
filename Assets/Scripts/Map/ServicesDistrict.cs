
public class ServicesDistrict : District
{
    public enum Subtype
    {
        Academy,
        Library,
        Hospice,
    }
    Subtype subtype;

    public override int DistrictCategoryIndex => (int)DistrictCategory.Services;
    public override int DistrictSubType => (int)subtype;

    public ServicesDistrict(int index, HexCell hexCell) : base("New Services District", hexCell)
    {
        subtype = (Subtype)index;
    }

    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
