using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.UIElements;

public abstract class Structure : MonoBehaviour
{
    [Header("Structure")]
    [SerializeField] ConstructionCost constructionCost = null;
    public ConstructionCost ConstructionCost { get => constructionCost; }
    [Header("References")]
    [SerializeField] Tilemap tmConstructed = null;
    [SerializeField] Tilemap tmConstructionArea = null;
    public Bar constructionProgressBar = null;

    [Header("Setup")]
    [SerializeField] protected int xSize = 1;
    [SerializeField] protected int ySize = 1;
    [SerializeField] protected float cellSize = 1;

    public ConstructionArea constructionArea = new ConstructionArea();
    protected City city;
    Vector3 rotationOffset;

    Direction facingDirection = Direction.Down;
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
    }

    public void Load(City city) => Constructed(city, false);

    public virtual void InteractedWith(Unit unitVisiting) { }
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
        Destroy(this.gameObject);
    }
}