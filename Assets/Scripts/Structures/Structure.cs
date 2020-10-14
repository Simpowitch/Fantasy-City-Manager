using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public abstract class Structure : MonoBehaviour
{
    //Delegates
    public delegate void UnitHandler(Unit unit);
    public event UnitHandler OnUnitVisiting;

    [Header("Structure")]
    [SerializeField] ConstructionCost constructionCost = null;
    public ConstructionArea constructionArea = null;

    public ConstructionCost ConstructionCost { get => constructionCost; }
    [Header("References")]
    [SerializeField] Tilemap groundTilemap = null;
    [SerializeField] Tilemap wallTilemap = null;
    public Bar constructionProgressBar = null;

    [Header("Setup")]
    [SerializeField] protected int xSize = 1;
    [SerializeField] protected int ySize = 1;
    float CellSize { get => city != null ? city.CellSize : 1f; }
    [SerializeField] Transform blockerObjectParent = null;
    ObjectTile[] objectBlockedTiles = null;
    [SerializeField] Transform finalPathPositionParent = null;
    ObjectTile[] finalPathTiles = null;

    List<ObjectTile> groundTiles;
    List<ObjectTile> wallTiles;
    List<ObjectTile> structureTiles;
    public List<ObjectTile> StructureTiles
    {
        get
        {
            if (analyzeTilesFlag)
                AnalyzeTiles();
            return structureTiles;
        }
        set
        {
            analyzeTilesFlag = false;
            structureTiles = value;
        }
    }
    bool analyzeTilesFlag = true;

    protected City city;
    Vector3 rotationOffset;

    [SerializeField] Facing facingDirection = Facing.Down;
    public Facing FacingDirecion
    {
        get => facingDirection;
        set
        {
            facingDirection = value;
            transform.eulerAngles = new Vector3(0, 0, -(int)value * 90);

            switch (value)
            {
                case Facing.Down:
                    rotationOffset = new Vector3(0, 0);
                    break;
                case Facing.Left:
                    rotationOffset = new Vector3(0, CellSize);
                    break;
                case Facing.Up:
                    rotationOffset = new Vector3(CellSize, CellSize);
                    break;
                case Facing.Right:
                    rotationOffset = new Vector3(CellSize, 0);
                    break;
            }
            transform.position = anchorPoint + rotationOffset;

            analyzeTilesFlag = true;
        }
    }

    Vector3 anchorPoint;
    public Vector3 AnchorPoint
    {
        set
        {
            anchorPoint = value;
            transform.position = value + rotationOffset;

            analyzeTilesFlag = true;
        }
    }

    public Vector2 LowerLeftCorner
    {
        get
        {
            switch (facingDirection)
            {
                case Facing.Down:
                    return anchorPoint + rotationOffset;
                case Facing.Left:
                    return anchorPoint + new Vector3(0, -xSize * CellSize) + rotationOffset;
                case Facing.Up:
                    return anchorPoint + new Vector3(-xSize * CellSize, -ySize * CellSize) + rotationOffset;
                case Facing.Right:
                    return anchorPoint + new Vector3(-ySize * CellSize, 0) + rotationOffset;
                default:
                    return anchorPoint;
            }
        }
    }

    public Vector2 UpperRightCorner
    {
        get
        {
            Vector2 corner = LowerLeftCorner;
            switch (facingDirection)
            {
                case Facing.Down:
                case Facing.Up:
                    return corner + new Vector2(xSize * CellSize, ySize * CellSize);
                case Facing.Left:
                case Facing.Right:
                    return corner + new Vector2(ySize * CellSize, xSize * CellSize);
                default:
                    return corner;
            }
        }
    }

    public Vector2 CenterPosition
    {
        get
        {
            Vector2 corner = LowerLeftCorner;
            switch (facingDirection)
            {
                case Facing.Down:
                case Facing.Up:
                    return corner + new Vector2(xSize * CellSize / 2, ySize * CellSize / 2);
                case Facing.Left:
                case Facing.Right:
                    return corner + new Vector2(ySize * CellSize / 2, xSize * CellSize / 2);
                default:
                    return corner;
            }
        }
    }

    protected List<Unit> unitsInside = new List<Unit>();

    //Called from the construction system when a new building is placed
    public void BuildConstructionArea(City city)
    {
        constructionProgressBar.gameObject.SetActive(true);
        SetTransparent(true);
        this.city = city;
        constructionArea.Setup(() => Constructed(city, true));
        city.AddConstructionArea(this);
        ChangeTiles(true);
    }

    //Is called when the construction progress is finished or when loaded
    protected virtual void Constructed(City city, bool addToCityList)
    {
        constructionProgressBar.gameObject.SetActive(false);
        SetTransparent(false);
        this.city = city;
        DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
        city.RemoveConstructionArea(this);
        ChangeTiles(true);
    }

    public void MakePreview(City city)
    {
        SetTransparent(true);
        this.city = city;
        AnalyzeTiles();
    }

    //Calculates all the different tiles occupied by objects, floors, walls etc.
    private void AnalyzeTiles()
    {
        StructureTiles = new List<ObjectTile>();
        groundTiles = new List<ObjectTile>();
        wallTiles = new List<ObjectTile>();

        List<ObjectTile> ObjectTiles = city.ObjectGrid.GetGridObjects(LowerLeftCorner, UpperRightCorner);

        //Ground Tiles
        if (groundTilemap != null)
        {
            Tilemap tilemap = groundTilemap;

            foreach (var tile in ObjectTiles)
            {
                Vector3Int cellTilemapPosition = tilemap.WorldToCell(tile.CenteredWorldPosition);
                if (tilemap.HasTile(cellTilemapPosition))
                {
                    groundTiles.Add(tile);
                    StructureTiles.Add(tile);
                }
            }
        }

        //Wall Tiles
        if (wallTilemap != null)
        {
            Tilemap tilemap = wallTilemap;

            foreach (var tile in ObjectTiles)
            {
                Vector3Int cellTilemapPosition = tilemap.WorldToCell(tile.CenteredWorldPosition);
                if (tilemap.HasTile(cellTilemapPosition))
                {
                    wallTiles.Add(tile);
                    StructureTiles.Add(tile);
                }
            }
        }

        //Blocker tiles
        objectBlockedTiles = new ObjectTile[blockerObjectParent != null ? blockerObjectParent.childCount : 0];
        for (int i = 0; i < objectBlockedTiles.Length; i++)
        {
            objectBlockedTiles[i] = city.ObjectGrid.GetGridObject(blockerObjectParent.GetChild(i).transform.position);
        }

        //Final path tiles
        finalPathTiles = new ObjectTile[finalPathPositionParent != null ? finalPathPositionParent.childCount : 0];
        for (int i = 0; i < finalPathTiles.Length; i++)
        {
            finalPathTiles[i] = city.ObjectGrid.GetGridObject(finalPathPositionParent.GetChild(i).transform.position);
        }
    }

    //Change tiles to walkable or not, and set tiles construction field to match this structure
    private void ChangeTiles(bool constructed)
    {
        //Change tiles construction field
        foreach (ObjectTile tile in StructureTiles)
        {
            tile.Structure = constructed ? this : null;
        }

        //Change pathfinding cost
        foreach (ObjectTile tile in groundTiles)
        {
            city.RoadNetwork.AddRoad(tile.CenteredWorldPosition, constructed ? RoadNetwork.GroundType.Indoor : RoadNetwork.GroundType.Grass);
        }

        //Set walls tiles to not walkable or free
        foreach (ObjectTile tile in wallTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.CenteredWorldPosition, constructed ? PathNode.MovementAllowance.Forbidden : PathNode.MovementAllowance.Free);
        }

        //Set tiles with blockerobjects to not walkable or free
        foreach (ObjectTile tile in objectBlockedTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.CenteredWorldPosition, constructed ? PathNode.MovementAllowance.Forbidden : PathNode.MovementAllowance.Free);
        }

        //Set tiles with final path positions to either finalpath marked or free
        foreach (ObjectTile tile in finalPathTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.CenteredWorldPosition, constructed ? PathNode.MovementAllowance.OnlyFinal : PathNode.MovementAllowance.Free);
        }
    }

    public void Load(City city) => Constructed(city, false);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unitCollision = collision.GetComponent<Unit>();
        if (unitCollision != null)
        {
            UnitVisiting(unitCollision);
            if (!unitsInside.Contains(unitCollision))
                unitsInside.Add(unitCollision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Unit unitCollision = collision.GetComponent<Unit>();
        if (unitCollision != null && unitsInside.Contains(unitCollision))
            unitsInside.Remove(unitCollision);
    }

    protected virtual void UnitVisiting(Unit unitVisiting)
    {
        OnUnitVisiting?.Invoke(unitVisiting);
    }

    public Vector3 GetRandomLocation() => Utility.ReturnRandom(groundTiles).CenteredWorldPosition;

    protected virtual void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay) { }

    public void SetTransparent(bool state)
    {
        GetComponent<Collider2D>().enabled = !state;
        Color color = state ? Color.grey : Color.white;
        color.a = state ? 0.4f : 1f;
        groundTilemap.color = color;
    }

    public void ChangeBuildable(bool canBeBuilt)
    {
        float alpha = groundTilemap.color.a;
        Color newColor = canBeBuilt ? Color.green : Color.red;
        newColor.a = alpha;
        groundTilemap.color = newColor;
    }

    public virtual void Despawn()
    {
        DayNightSystem.OnPartOfTheDayChanged -= PartOfDayChange;

        ChangeTiles(false);

        Destroy(this.gameObject);
    }
}