using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class HexCell : MonoBehaviour
{
    [Header("Canvas and Text")]
    public GameObject labelCanvas;
    public Text numberLabel;
    public Text nameLabel;

    [Header("References")]
    [SerializeField] SpriteRenderer gameGrid = null;
    [SerializeField] Transform featureParent = null;
    [Header("Visual outlines and markers")]
    [SerializeField] SpriteRenderer selectionRenderer = null;

    public HexCoordinates coordinates;
    public HexGrid myGrid;

    bool traversable = false;
    public bool Traversable
    {
        get => traversable;
        set
        {
            traversable = value;
        }
    }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    //DELEGATES
    public delegate void HexCellHandler(HexCell cell);
    public static HexCellHandler OnHexCellHoover;
    public static HexCellHandler OnHexCellClicked;

    public List<HexUnit> UnitsOnCell { get; private set; } = new List<HexUnit>();

    public District District { get; set; }

    #region Terrain and Features
    public bool IsLand { get; set; }
    public bool IsOcean { get => !IsLand; }
    public bool IsShore { get => IsLand && Bitmask >= 0 && Bitmask <= 62; }
    public int Bitmask { get; set; }
    public void CalculateBitmask()
    {
        if (IsOcean)
        {
            Bitmask = -1;
            return;
        }
        int index = 0;
        short bitValue = 1;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = GetNeighbor(d);
            if (neighbor)
            {
                if (neighbor.IsLand)
                {
                    index += bitValue;
                }
            }
            else
                index += bitValue;
            bitValue *= 2;
        }
        Bitmask = index;
    }

    public void SpawnStructures(int numbersToSpawn, Structure[] structurePool)
    {
        for (int i = 0; i < District.Structures.Count; i++)
        {
            GameObject.Destroy(District.Structures[i]);
            i--;
        }
        for (int i = 0; i < numbersToSpawn; i++)
        {
            Structure newHouse = Instantiate(Utility.ReturnRandom(structurePool), featureParent);
            newHouse.transform.position = Position;
            StartCoroutine(PlaceFeature(newHouse.transform));
            District.Structures.Add(newHouse);
            newHouse.SetDistrict(District);
        }
    }

    const int MAXTEST = 10000;
    IEnumerator PlaceFeature(Transform feature)
    {
        int test = 0;
        bool allowed = false;
        Collider2D thisCollider = feature.GetComponent<Collider2D>();

        while (test < MAXTEST && !allowed)
        {
            test++;
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            Vector3 normalizedDirection = new Vector3(x, y).normalized;
            Vector3 offset = normalizedDirection * Random.Range(0.1f, HexMetrics.innerRadius);
            Vector3 testPosition = Position + offset;

            feature.localRotation = Quaternion.Euler(0f, 0f, 360f * Random.value);
            feature.position = testPosition;
            yield return new WaitForFixedUpdate();

            allowed = true;
            foreach (var structure in District.Structures)
            {
                if (structure.GetComponent<Collider2D>().IsTouching(thisCollider))
                {
                    Debug.Log("Collision detected");
                    allowed = false;
                    break;
                }
            }
        }
        if (allowed)
            Debug.Log("Success");
        else
            Debug.Log("Failed");
    }
    #endregion

    #region Neighbors
    [SerializeField]
    public HexCell[] Neighbors { get; } = new HexCell[6];

    public HexCell GetNeighbor(HexDirection direction)
    {
        return Neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        HexCell neighbor = Neighbors[(int)direction]; //Old neighbor
        if (neighbor)
        {
            neighbor.Neighbors[(int)direction.Opposite()] = null;
        }
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }
    #endregion

    #region Pathfinding
    public void ClearPathfinding()
    {
        SearchHeuristic = 0;
        NextWithSamePriority = null;
        SearchPhase = 0;
        MovementCost = 0;
        PathFrom = null;
    }

    public int SearchHeuristic { get; set; }
    public int SearchPriority
    {
        get
        {
            return MovementCost + SearchHeuristic;
        }
    }
    public HexCell NextWithSamePriority { get; set; }
    public int SearchPhase { get; set; } // 0 = not been reached | 1 = currently in searchfrontier | 2 = has been reached and taken out from frontier

    public int MovementCost { get; set; } //The total cost to move here by a single unit now searching
    public HexCell PathFrom { get; set; }
    public int BaseEnterModifier { get; set; }
    #endregion


    #region Grid, Highlights and Labels
    public void SetNumberLabel(string text)
    {
        numberLabel.text = text;
        CheckCanvasActiveNeed();
    }
    public void SetNameLabel(string text)
    {
        nameLabel.text = text;
        CheckCanvasActiveNeed();
    }
    private void CheckCanvasActiveNeed() => labelCanvas.SetActive(numberLabel.text != "" || nameLabel.text != "");
    public void ShowUI(bool status)
    {
        numberLabel.enabled = status;
        nameLabel.enabled = status;
    }

    public void ShowSelected(bool status) => selectionRenderer.enabled = status;

    public void ShowGameOutline(bool status)
    {
        if (Traversable)
        {
            gameGrid.enabled = status;
        }
        else
        {
            gameGrid.enabled = false;
        }
    }
    #endregion

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            OnHexCellHoover?.Invoke(this);
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            OnHexCellClicked?.Invoke(this);
    }

    #region Save and Load
    public void Load(HexCellData data, HexGrid grid)
    {
        Traversable = data.traversable;
        IsLand = data.isLand;
        Bitmask = data.bitmask;

        for (int i = 0; i < data.connected.Length; i++)
        {
            if (data.connected[i])
            {
                SetNeighbor((HexDirection)i, grid.GetCell(data.connectedCoordinates[i]));
            }
        }
    }
    #endregion
}