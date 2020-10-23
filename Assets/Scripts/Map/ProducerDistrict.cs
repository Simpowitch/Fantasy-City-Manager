public class ProducerDistrict : District
{
    public enum Subtype
    {
        Farm,
        Forestry,
        Mine,
        Quarry
    }
    Subtype subtype;

    public override int DistrictCategoryIndex => (int)DistrictCategory.Producer;
    public override int DistrictSubType => (int)subtype;

    public ProducerDistrict(int index, HexCell hexCell) : base("New Producer District", hexCell)
    {
        subtype = (Subtype)index;
    }

    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
