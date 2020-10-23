public class TradeDistrict : District
{
    public enum Subtype
    {
        Marketplace,
        Harbor,
        FoodAndDrinks,
        Artisan
    }
    Subtype subtype;

    public override int DistrictCategoryIndex => (int)DistrictCategory.Producer;
    public override int DistrictSubType => (int)subtype;

    public TradeDistrict(int index, HexCell hexCell) : base("New Trade District", hexCell)
    {
        subtype = (Subtype)index;
    }

    public override string GetCurrentInformation()
    {
        return subtype.ToString();
    }
}
