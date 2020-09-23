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
    [SerializeField] Tilemap tmConstructed = null;
    [SerializeField] Tilemap tmConstructionArea = null;
    public Bar constructionProgressBar = null;

    [Header("Setup")]
    [SerializeField] protected int xSize = 1;
    [SerializeField] protected int ySize = 1;
    [SerializeField] protected float cellSize = 1;

    [Header("Floor setup")]
    [SerializeField] Transform[] wallTiles = null;

    protected City city;
    Vector3 rotationOffset;

    [SerializeField] Direction facingDirection = Direction.Down;
    public Direction FacingDirecion
    {
        get => facingDirection;
        set
        {
            facingDirection = value;
            transform.eulerAngles = new Vector3(0, 0, -(int)value * 90);

            switch (value)
            {
                case Direction.Down:
                    rotationOffset = new Vector3(0, 0);
                    break;
                case Direction.Left:
                    rotationOffset = new Vector3(0, cellSize);
                    break;
                case Direction.Up:
                    rotationOffset = new Vector3(cellSize, cellSize);
                    break;
                case Direction.Right:
                    rotationOffset = new Vector3(cellSize, 0);
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
                case Direction.Down:
                    return anchorPoint + rotationOffset;
                case Direction.Left:
                    return anchorPoint + new Vector3(0, -xSize * cellSize) + rotationOffset;
                case Direction.Up:
                    return anchorPoint + new Vector3(-xSize * cellSize, -ySize * cellSize) + rotationOffset;
                case Direction.Right:
                    return anchorPoint + new Vector3(-ySize * cellSize, 0) + rotationOffset;
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
                case Direction.Down:
                case Direction.Up:
                    return corner + new Vector2(xSize * cellSize, ySize * cellSize);
                case Direction.Left:
                case Direction.Right:
                    return corner + new Vector2(ySize * cellSize, xSize * cellSize);
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
                case Direction.Down:
                case Direction.Up:
                    return corner + new Vector2(xSize * cellSize / 2, ySize * cellSize / 2);
                case Direction.Left:
                case Direction.Right:
                    return corner + new Vector2(ySize * cellSize / 2, xSize * cellSize / 2);
                default:
                    return corner;
            }
        }
    }
    public List<ObjectTile> ObjectTiles { get => city.ObjectGrid.GetGridObjects(LowerLeftCorner, UpperRightCorner); }

    protected List<Unit> unitsInside = new List<Unit>();

    public void BuildConstructionArea(City city)
    {
        constructionProgressBar.gameObject.SetActive(true);
        this.city = city;
        constructionArea.Setup(() => Constructed(city, true));
        tmConstructed.gameObject.SetActive(false);
        tmConstructionArea.gameObject.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        city.AddConstructionArea(this);
    }

    protected virtual void Constructed(City city, bool addToCityList)
    {
        constructionProgressBar.gameObject.SetActive(false);
        this.city = city;
        DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
        tmConstructed.gameObject.SetActive(true);
        tmConstructionArea.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = true;
        city.RemoveConstructionArea(this);

        FloorSetup();
    }

    private void FloorSetup()
    {
        //Change pathfinding cost to indoor cost of all tiles
        foreach (ObjectTile tile in ObjectTiles)
        {
            city.RoadNetwork.AddRoad(tile.CenteredWorldPosition, RoadNetwork.GroundType.Indoor);
        }


        //Set walls to non-walkable tiles
        foreach (Transform tile in wallTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.position, false);
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

    public Vector3 GetRandomLocation() => Utility.ReturnRandom(ObjectTiles).CenteredWorldPosition;


    protected virtual void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay) { }

    public void SetTransparent()
    {
        GetComponent<Collider2D>().enabled = false;
        Color color = tmConstructed.color;
        color.a = 0.5f;
        tmConstructed.color = color;
    }

    public void ChangeBuildable(bool canBeBuilt)
    {
        float alpha = tmConstructed.color.a;
        Color newColor = canBeBuilt ? Color.green : Color.red;
        newColor.a = alpha;
        tmConstructed.color = newColor;
    }

    public virtual void Despawn()
    {
        DayNightSystem.OnPartOfTheDayChanged -= PartOfDayChange;

        //Change pathfinding cost to outdoor cost of all tiles
        foreach (var item in ObjectTiles)
        {
            city.RoadNetwork.AddRoad(item.CenteredWorldPosition, RoadNetwork.GroundType.Grass);
        }

        //Set walls to walkable tiles
        foreach (Transform tile in wallTiles)
        {
            city.RoadNetwork.ChangeWalkable(tile.position, true);
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