
public class ResidentialDistrict : District
{
    public enum Subtype
    {
        Squalid,
        Poor,
        Comfortable,
        Wealthy,
        Artistocratic
    }
    Subtype subtype;


    public override int DistrictCategoryIndex => (int)DistrictCategory.Residential;
    public override int DistrictSubType => (int)subtype;

    int[] houseCapacity = new int[] { 16, 16, 8, 8, 4 };

    public ResidentialDistrict(int index, HexCell hexCell) : base("New Residential District", hexCell)
    {
        subtype = (Subtype)index;
        HouseCapacity = houseCapacity[index];
    }

    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
