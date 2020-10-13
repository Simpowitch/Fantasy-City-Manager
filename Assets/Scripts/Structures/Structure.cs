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

    List<ObjectTile> groundTiles;
    List<ObjectTile> wallTiles;
    public List<ObjectTile> StructureTiles { get; private set; }

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
        }
    }

    Vector3 anchorPoint;
    public Vector3 AnchorPoint
    {
        set
        {
            anchorPoint = value;
            transform.position = value + rotationOffset;
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

    public void BuildConstructionArea(City city)
    {
        constructionProgressBar.gameObject.SetActive(true);
        SetTransparent(true);
        this.city = city;
        constructionArea.Setup(() => Constructed(city, true));
        city.AddConstructionArea(this);
        SetupObjectTiles();
    }

    protected virtual void Constructed(City city, bool addToCityList)
    {
        constructionProgressBar.gameObject.SetActive(false);
        SetTransparent(false);
        this.city = city;
        DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
        city.RemoveConstructionArea(this);


        if (StructureTiles == null)
            SetupObjectTiles();

        //Change pathfinding cost to indoor cost of all tiles
        foreach (ObjectTile tile in groundTiles)
        {
            city.RoadNetwork.AddRoad(tile.CenteredWorldPosition, RoadNetwork.GroundType.Indoor);
        }

        //Set walls to non-walkable tiles
        foreach (ObjectTile tile in wallTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.CenteredWorldPosition, false);
        }
    }

    private void SetupObjectTiles()
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

        //Change pathfinding cost to indoor cost of all tiles
        foreach (ObjectTile tile in groundTiles)
        {
            city.RoadNetwork.AddRoad(tile.CenteredWorldPosition, RoadNetwork.GroundType.Grass);
        }

        //Set walls to non-walkable tiles
        foreach (ObjectTile tile in wallTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.CenteredWorldPosition, true);
        }

        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Low Left to Up Left
        Vector3 posA = LowerLeftCorner;
        Vector3 posB = new Vector2(LowerLeftCorner.x, UpperRightCorner.y);
        Gizmos.DrawLine(posA, posB);

        //Up Left to Up Right
        posA = posB;
        posB = UpperRightCorner;
        Gizmos.DrawLine(posA, posB);

        //Up Right to Low Right
        posA = posB;
        posB = new Vector2(UpperRightCorner.x, LowerLeftCorner.y);
        Gizmos.DrawLine(posA, posB);

        //Low Right to Low Left
        posA = posB;
        posB = LowerLeftCorner;
        Gizmos.DrawLine(posA, posB);
    }
}