using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Facing { Down, Left, Up, Right, MAX = 3, Invalid }
public class ConstructionSystem : MonoBehaviour
{
    public enum Mode { Off, Road, Structure }
    Mode constructionMode = Mode.Off;
    public Mode ConstructionMode
    {
        get => constructionMode;
        set
        {
            constructionMode = value;
            if (value == Mode.Off)
                constructionInformationPanel.SetActive(false);
        }
    }

    [Header("Static references")]
    [SerializeField] Player player = null;
    City city;
    Grid<ObjectTile> ObjectGrid { get => city.ObjectGrid; }
    RoadNetwork RoadNetwork { get => city.RoadNetwork; }
    [SerializeField] TextSetter constructionInformation = null;
    [SerializeField] GameObject constructionInformationPanel = null;

    [Header("Roads")]
    [SerializeField] Tile[] roadTiles = null;
    [SerializeField] ConstructionCost[] roadCosts = null;
    [Header("Buildings")]
    [SerializeField] Structure[] structures = null;

    [Header("Tilemaps")]
    [SerializeField] Tilemap constructionPreview = null;
    [SerializeField] Tilemap roadTilemap = null;

    [Header("Parents")]
    [SerializeField] Transform structureParent = null;

    List<Vector3Int> roadPreviews = new List<Vector3Int>();
    Structure structurePreview;
    int constructionIndex = 0;
    Facing constructionFacing;

    private void Awake()
    {
        player.OnActiveCityChanged += SetCity;
    }

    private void SetCity(City city)
    {
        this.city = city;
    }

    public void SetConstructionIndex(int index)
    {
        constructionIndex = index;
        constructionInformationPanel.SetActive(constructionMode != Mode.Off);
        switch (constructionMode)
        {
            case Mode.Off:
                break; 
            case Mode.Road:
                constructionInformation.SetText(GetRoadCostString());
                break;
            case Mode.Structure:
                constructionInformation.SetText(GetStructureCostString());
                break;
        }
    }

    public void ChangeFacingRotationDirection()
    {
        if ((int)constructionFacing + 1 > (int)Facing.MAX)
        {
            constructionFacing = 0;
            if (structurePreview)
                structurePreview.FacingDirecion = constructionFacing;
        }
        else
        {
            constructionFacing++;
            if (structurePreview)
                structurePreview.FacingDirecion = constructionFacing;
        }
    }

    public void SetFacingRotationDirection(Facing newDirection)
    {
        constructionFacing = newDirection;
        if (structurePreview)
            structurePreview.FacingDirecion = constructionFacing;
    }

    #region Previews & Allowed Construction Checks
    public void ShowRoadPreview(Vector3 tilePosition)
    {
        Vector3Int cellTilemapPosition = constructionPreview.WorldToCell(tilePosition);
        constructionPreview.SetTile(cellTilemapPosition, roadTiles[constructionIndex]);

        if (!roadPreviews.Contains(cellTilemapPosition))
        {
            roadPreviews.Add(cellTilemapPosition);
        }
    }
    public void ShowStructurePreview(Vector3 worldPosition)
    {
        if (!structurePreview) //Spawn preview if not already existing
        {
            Structure spawnedBuilding = Instantiate(structures[constructionIndex], structureParent);
            structurePreview = spawnedBuilding;
            structurePreview.SetTransparent();
        }
        //Move preview object
        Vector3 lowerLeftPosition = ObjectGrid.GetWorldPosition(worldPosition, false);
        structurePreview.FacingDirecion = constructionFacing;
        structurePreview.AnchorPoint = lowerLeftPosition;
    }

    public void ClearPreviews()
    {
        foreach (var item in roadPreviews)
        {
            constructionPreview.SetTile(item, null);
        }
        roadPreviews.Clear();
        if (structurePreview)
        {
            Destroy(structurePreview.gameObject);
        }
    }

    public bool CanConstructPreviewRoad(out string explanation, out string constructionCost)
    {
        bool canConstruct = true;
        explanation = "";
        if (roadPreviews == null)
        {
            if (explanation != "")
            {
                explanation += "\n";
            }
            explanation += "Improper location";
            canConstruct = false;
        }
        foreach (var item in roadPreviews)
        {
            Vector3 worldPosition = roadTilemap.CellToWorld(item);
            ObjectTile objectTile = ObjectGrid.GetGridObject(worldPosition);
            if (!objectTile.IsFree)
            {
                if (explanation != "")
                {
                    explanation += "\n";
                }
                explanation += "Location occupied";
                canConstruct = false;
                break;
            }
        }
        ConstructionCost costGroup = new ConstructionCost();
        foreach (var cost in roadCosts[constructionIndex].ResourceCosts)
        {
            CityResource resourceToAdd = new CityResource(cost.type, cost.Value * roadPreviews.Count);
            costGroup.AddResource(resourceToAdd);
        }
        bool canMeetCosts = CheckCosts(costGroup, out constructionCost);
        if (!canMeetCosts)
        {
            if (explanation != "")
            {
                explanation += "\n";
            }
            explanation += "Not enough resources stored";
            canConstruct = false;
        }
        if (explanation != "")
            explanation += "\n";
        return canConstruct;
    }
    public bool CanConstructPreviewStructure(out string explanation, out string constructionCost)
    {
        bool canConstruct = true;
        List<ObjectTile> objectTiles = ObjectGrid.GetGridObjects(structurePreview.LowerLeftCorner, structurePreview.UpperRightCorner);
        explanation = "";
        if (objectTiles == null)
        {
            if (explanation != "")
            {
                explanation += "\n";
            }
            explanation += "Improper location";
            canConstruct = false;
        }
        foreach (ObjectTile tile in objectTiles)
        {
            if (!tile.IsFree)
            {
                if (explanation != "")
                {
                    explanation += "\n";
                }
                explanation += "Location occupied";
                canConstruct = false;
                break;
            }
        }
        bool canMeetCosts = CheckCosts(structurePreview.ConstructionCost, out constructionCost);
        if (!canMeetCosts)
        {
            if (explanation != "")
            {
                explanation += "\n";
            }
            explanation += "Not enough resources stored";
            canConstruct = false;
        }
        if (explanation != "")
            explanation += "\n";
        structurePreview.ChangeBuildable(canConstruct);
        return canConstruct;
    }

    private bool CheckCosts(ConstructionCost costGroup, out string constructionCost)
    {
        bool canMeetCosts = true;
        constructionCost = "Costs:";
        foreach (var cost in costGroup.ResourceCosts)
        {
            int difference = cost.Value - city.cityStats.Inventory.GetCityResourceOfType(cost.type).Value;
            if (constructionCost != "")
                constructionCost += "\n";
            constructionCost += $"{cost.Value} {cost.type}";
            if (difference > 0)
            {
                constructionCost += $" (Missing {difference} {cost.type})";
                canMeetCosts = false;
            }
        }
        return canMeetCosts;
    }

    public string GetStructureCostString() => structures[constructionIndex].ConstructionCost.GetCostString();
    public string GetRoadCostString() => roadCosts[constructionIndex].GetCostString();
    #endregion

    #region Roads
    public void BuildRoad(Vector3 tilePosition)
    {
        Vector3Int cellTilemapPosition = roadTilemap.WorldToCell(tilePosition);
        roadTilemap.SetTile(cellTilemapPosition, roadTiles[constructionIndex]);
        RoadNetwork.AddRoad(tilePosition, (RoadNetwork.GroundType) constructionIndex);

        city.cityStats.Inventory.TryToRemove(roadCosts[constructionIndex].ResourceCosts);
    }

    public void RemoveRoad(Vector3 tilePosition)
    {
        Vector3Int cellTilemapPosition = roadTilemap.WorldToCell(tilePosition);
        roadTilemap.SetTile(cellTilemapPosition, null);
        RoadNetwork.RemoveRoad(tilePosition);
    }
    #endregion

    #region Structures
    public void BuildStructure(Vector3 worldPosition)
    {
        Vector3 lowerLeftPosition = ObjectGrid.GetWorldPosition(worldPosition, false);
        Structure spawnedBuilding = Instantiate(structures[constructionIndex], structureParent);
        spawnedBuilding.AnchorPoint = lowerLeftPosition;
        spawnedBuilding.FacingDirecion = constructionFacing;

        spawnedBuilding.BuildConstructionArea(city);

        List<ObjectTile> objectTiles = ObjectGrid.GetGridObjects(spawnedBuilding.LowerLeftCorner, spawnedBuilding.UpperRightCorner);
        foreach (ObjectTile item in objectTiles)
        {
            item.Structure = spawnedBuilding;
        }
        city.cityStats.Inventory.TryToRemove(structurePreview.ConstructionCost.ResourceCosts);
    }

    public void RemoveStructure(Vector3 centeredWorldPosition)
    {
        ObjectTile objectTile = ObjectGrid.GetGridObject(centeredWorldPosition);
        Structure foundBuilding = objectTile.Structure;
        if (foundBuilding != null)
        {
            List<ObjectTile> objectTiles = ObjectGrid.GetGridObjects(foundBuilding.LowerLeftCorner, foundBuilding.UpperRightCorner);
            foreach (ObjectTile item in objectTiles)
            {
                item.Structure = null;
            }
            foundBuilding.Despawn();
        }
    }
    #endregion
}

[System.Serializable]
public class ConstructionCost
{
    [SerializeField]
    List<CityResource> resourceCosts = new List<CityResource>();
    public List<CityResource> ResourceCosts { get => resourceCosts; }

    public void AddResource(CityResource newResource)
    {
        foreach (var cost in ResourceCosts)
        {
            if (cost.type == newResource.type)
            {
                cost.Value += newResource.Value;
                break;
            }
        }
        ResourceCosts.Add(newResource);
    }

    public string GetCostString()
    {
        string s = "";
        foreach (var cost in ResourceCosts)
        {
            if (s != "")
                s += "\n";
            s += $"{cost.Value} {cost.type}";
        }
        return s;
    }
}