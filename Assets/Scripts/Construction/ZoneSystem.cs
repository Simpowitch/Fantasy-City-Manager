//using System.Collections.Generic;

//public class ZoneSystem
//{
//    Grid<ObjectTile> objectGrid;
//    List<ObjectTile> highlighted;
//    ConstructionSystem constructionSystem;
//    public List<ObjectTile> ZonedTiles
//    {
//        get
//        {
//            List<ObjectTile> zoned = new List<ObjectTile>();
//            List<ObjectTile> allTiles = new List<ObjectTile>();
//            foreach (var tile in objectGrid.gridArray)
//            {
//                allTiles.Add(tile);
//            }
//            zoned.PopulateListWithMatchingConditions(allTiles, (tile) => tile.IsZonable, (tile) => tile.ResidentialZoned);
//            return zoned;
//        }
//    }

//    public ZoneSystem(Grid<ObjectTile> objectGrid, ConstructionSystem constructionSystem)
//    {
//        this.objectGrid = objectGrid;
//        this.constructionSystem = constructionSystem;
//    }


//    public void ShowZones(bool status)
//    {
//        if (highlighted != null)
//        {
//            foreach (var objectTile in highlighted)
//            {
//                objectTile.CanvasTileObject.SetZoneFillState(CanvasTile.ZoneFillState.Off);
//            }
//        }
//        if (!status)
//        {
//            highlighted = null;
//            return;
//        }
//        List<ObjectTile> zonable = new List<ObjectTile>();
//        List<ObjectTile> zoned = new List<ObjectTile>();

//        List<ObjectTile> allTiles = new List<ObjectTile>();
//        foreach (var tile in objectGrid.gridArray)
//        {
//            allTiles.Add(tile);
//        }

//        zonable.PopulateListWithMatchingConditions(allTiles, (tile) => tile.IsZonable, (tile) => !tile.ResidentialZoned);
//        zoned.PopulateListWithMatchingConditions(allTiles, (tile) => tile.IsZonable, (tile) => tile.ResidentialZoned);

//        highlighted = new List<ObjectTile>();
//        highlighted.AddRange(zonable);
//        highlighted.AddRange(zoned);

//        if (zonable != null)
//        {
//            foreach (var objectTile in zonable)
//            {
//                objectTile.CanvasTileObject.SetZoneFillState(CanvasTile.ZoneFillState.Zonable);
//            }
//        }
//        if (zoned != null)
//        {
//            foreach (var objectTile in zoned)
//            {
//                objectTile.CanvasTileObject.SetZoneFillState(CanvasTile.ZoneFillState.Zoned);
//            }
//        }
//    }
//}
