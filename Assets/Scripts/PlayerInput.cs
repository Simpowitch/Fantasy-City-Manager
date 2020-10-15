﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [Header("Static references")]
    [SerializeField] Player player = null;
    [SerializeField] ConstructionSystem constructionSystem = null;
    [SerializeField] MouseTooltip mouseTooltip = null;
    City city;
    Grid<ObjectTile> ObjectGrid { get => city.ObjectGrid; }
    [SerializeField] ObjectViewer objectViewer = null;
    [SerializeField] InGameCamera playerCamera = null;
    [SerializeField] PlayerCharacter playerCharacter = null;
    [SerializeField] GameObject UI = null;

    [Header("Selection")]
    [SerializeField] GameObject selectionObject = null;
    [SerializeField] SpriteRenderer modeObjectRenderer = null;
    [SerializeField] CanvasGrid canvasGrid = null;
    [Header("Selection Sprites")]
    [SerializeField] Sprite normal = null;
    [SerializeField] Sprite build = null;
    [SerializeField] Sprite remove = null;


    Vector3 mouseDownPosition;

    List<ObjectTile> selectedObjectTiles;

    bool canMultiSelect = false;
    public enum InputMode { Selection, BuildRoad, BuildStructure, RemoveRoad, RemoveStructure, Harvest, CancelHarvest, MAX = 6 }
    InputMode mode;
    InputMode Mode
    {
        get => mode;
        set
        {
            mode = value;
            AllowedConstruction = true;
            constructionSystem.ClearPreviews();
            canvasGrid.SetGridOutlines(value != InputMode.Selection);
            Color modeObjectColor = modeObjectRenderer.color;
            switch (value)
            {
                case InputMode.Selection:
                    canMultiSelect = false;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = normal;
                    mouseTooltip.Hide();
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case InputMode.BuildRoad:
                    canMultiSelect = true;
                    modeObjectRenderer.sprite = build;
                    modeObjectColor.a = 0.5f;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Road;
                    break;
                case InputMode.BuildStructure:
                    canMultiSelect = false;
                    modeObjectRenderer.sprite = build;
                    modeObjectColor.a = 0.5f;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Structure;
                    break;
                case InputMode.RemoveRoad:
                    canMultiSelect = true;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = remove;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case InputMode.RemoveStructure:
                    canMultiSelect = false;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = remove;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case InputMode.Harvest:
                case InputMode.CancelHarvest:
                    canMultiSelect = true;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = build;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
            }
            modeObjectRenderer.color = modeObjectColor;
        }
    }

    bool allowedConstruction;
    bool AllowedConstruction
    {
        get => allowedConstruction;
        set
        {
            allowedConstruction = value;
            if (selectedObjectTiles != null)
            {
                foreach (var objectTile in selectedObjectTiles)
                {
                    objectTile.CanvasTileObject.SetSelectionFillState(value ? CanvasTile.SelectionFillState.Allowed : CanvasTile.SelectionFillState.Disallowed);
                }
            }
        }
    }

    bool gamePaused = false;


    private void Awake()
    {
        player.OnActiveCityChanged += SetCity;
        playerCamera.SetTrackingTarget(playerCharacter.transform);
    }

    private void SetCity(City city)
    {
        this.city = city;
    }

    // Update is called once per frame
    void Update()
    {
        if (!city)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Hoover();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            LeftClickDown();
        else if (Input.GetKey(KeyCode.Mouse0))
            LeftClickDrag();
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            LeftClickUp();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            Mode = InputMode.Selection;

        if (Input.GetKeyDown(KeyCode.R))
            constructionSystem.ChangeFacingRotationDirection();

        if (Input.GetKeyDown(KeyCode.C))
        {
            playerCamera.ToggleTracking();
            playerCharacter.CanMove = playerCamera.IsTracking;
        }

        if (Input.GetKeyDown(KeyCode.P))
            PauseGame(!gamePaused);
        if (Input.GetKeyDown(KeyCode.U))
            ToggleUIState();
    }

    void Hoover()
    {
        Vector3 centeredWorldPosition = ObjectGrid.GetWorldPosition(Utility.GetMouseWorldPosition(), true);

        //Debug.Log(ObjectGrid.GetGridObject(centeredWorldPosition).ToString());
        //Debug.Log(Utility.GetDirection(playerCharacter.transform.position, centeredWorldPosition));

        selectionObject.transform.position = centeredWorldPosition;
        switch (Mode)
        {
            case InputMode.Selection:
                break;
            case InputMode.BuildRoad:
                constructionSystem.ClearPreviews();
                if (selectedObjectTiles != null)
                {
                    foreach (var item in selectedObjectTiles)
                    {
                        constructionSystem.ShowRoadPreview(item.CenteredWorldPosition);
                    }
                }
                else
                {
                    constructionSystem.ShowRoadPreview(centeredWorldPosition);
                }
                AllowedConstruction = constructionSystem.CanConstructPreviewRoad(out string roadExplanation, out string roadConstructionCost);
                string roadTooltip = "";
                if (roadExplanation != "")
                    roadTooltip += roadExplanation;
                if (roadConstructionCost != "")
                {
                    if (roadTooltip != "")
                        roadTooltip += "\n";
                    roadTooltip += roadConstructionCost;
                }
                mouseTooltip.SetUp(AllowedConstruction ? MouseTooltip.ColorText.Allowed : MouseTooltip.ColorText.Forbidden, roadTooltip);
                break;
            case InputMode.RemoveRoad:
                break;
            case InputMode.BuildStructure:
                constructionSystem.ShowStructurePreview(centeredWorldPosition);
                AllowedConstruction = constructionSystem.CanConstructPreviewStructure(out string structureExplanation, out string structureConstructionCost);
                string structureTooltip = "";
                if (structureExplanation != "")
                    structureTooltip += structureExplanation;
                if (structureConstructionCost != "")
                {
                    if (structureTooltip != "")
                        structureTooltip += "\n";
                    structureTooltip += structureConstructionCost;
                }
                mouseTooltip.SetUp(AllowedConstruction ? MouseTooltip.ColorText.Allowed : MouseTooltip.ColorText.Forbidden, structureTooltip);
                break;
            case InputMode.RemoveStructure:
                break;
        }
    }

    void LeftClickDown() => mouseDownPosition = Utility.GetMouseWorldPosition();

    void LeftClickDrag()
    {
        if (!canMultiSelect)
        {
            return;
        }
        GetMouseSelectedArea(out Vector3 lowerLeft, out Vector3 upperRight);

        SetNewSelectedObjectTiles(ObjectGrid.GetGridObjects(lowerLeft, upperRight));
    }

    void LeftClickUp()
    {
        Vector3 worldPosition = Utility.GetMouseWorldPosition();
        //Handle changes
        switch (Mode)
        {
            case InputMode.Selection:
                IViewable unit = GetViewableUnderMouse();
                if (unit != null)
                    objectViewer.ShowViewable(unit);
                else
                    objectViewer.Hide();
                break;
            case InputMode.BuildRoad:
                if (allowedConstruction)
                {
                    foreach (var objectTile in selectedObjectTiles)
                    {
                        constructionSystem.BuildRoad(objectTile.CenteredWorldPosition);
                    }
                }
                break;
            case InputMode.RemoveRoad:
                if (allowedConstruction)
                {
                    foreach (var objectTile in selectedObjectTiles)
                    {
                        constructionSystem.RemoveRoad(objectTile.CenteredWorldPosition);
                    }
                }
                break;
            case InputMode.BuildStructure:
                if (allowedConstruction)
                {
                    constructionSystem.BuildStructure(worldPosition);
                }
                break;
            case InputMode.RemoveStructure:
                if (allowedConstruction)
                {
                    constructionSystem.RemoveStructure(worldPosition);
                }
                break;
            case InputMode.Harvest:
                foreach (var objectTile in selectedObjectTiles)
                {
                    ResourceObject resourceObject = objectTile.ResourceObject;
                    if (resourceObject != null && resourceObject.CanBeHarvested && resourceObject.HarvestMode == ResourceObject.HarvestMarkMode.Manual)
                        resourceObject.MarkForHarvest(true);
                }
                break;
            case InputMode.CancelHarvest:
                foreach (var objectTile in selectedObjectTiles)
                {
                    ResourceObject resourceObject = objectTile.ResourceObject;
                    if (resourceObject != null && resourceObject.CanBeHarvested && resourceObject.HarvestMode == ResourceObject.HarvestMarkMode.Manual)
                        resourceObject.MarkForHarvest(false);
                }
                break;
        }
        //Reset selections
        SetNewSelectedObjectTiles(null);

        //Reset modes
        switch (Mode)
        {
            case InputMode.Selection:
                break;
            case InputMode.BuildRoad:
            case InputMode.RemoveRoad:
            case InputMode.BuildStructure:
            case InputMode.RemoveStructure:
                constructionSystem.ClearPreviews();
                break;
        }
    }

    public void SetMode(int intMode)
    {
        if (intMode < 0 || intMode > (int)InputMode.MAX)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        Mode = (InputMode)intMode;
    }

    public void SetNewSelectedObjectTiles(List<ObjectTile> newObjectTiles)
    {
        if (selectedObjectTiles != null)
        {
            foreach (var objectTile in selectedObjectTiles)
            {
                objectTile.CanvasTileObject.SetSelectionFillState(CanvasTile.SelectionFillState.Off);
            }
        }

        if (newObjectTiles != null)
        {
            foreach (var objectTile in newObjectTiles)
            {
                objectTile.CanvasTileObject.SetSelectionFillState(AllowedConstruction ? CanvasTile.SelectionFillState.Allowed : CanvasTile.SelectionFillState.Disallowed);
            }
        }
        selectedObjectTiles = newObjectTiles;
    }

    private List<Unit> GetUnitsInselectedArea(Vector3 lowerLeft, Vector3 upperRight)
    {
        List<Unit> units = new List<Unit>();
        Collider2D[] allColliders = Physics2D.OverlapBoxAll((lowerLeft + upperRight / 2), upperRight - lowerLeft, 0);
        foreach (var collider in allColliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit)
                units.Add(unit);
        }
        return units;
    }

    private IViewable GetViewableUnderMouse()
    {
        ObjectTile objectTile = ObjectGrid.GetGridObject(Utility.GetMouseWorldPosition());
        if (objectTile != null)
        {
            Collider2D[] allColliders = Physics2D.OverlapBoxAll(objectTile.CenteredWorldPosition, new Vector2(ObjectGrid.cellSize / 2, ObjectGrid.cellSize / 2), 0);
            foreach (var collider in allColliders)
            {
                IViewable viewable = collider.GetComponent<IViewable>();
                if (viewable != null)
                    return viewable;
            }
        }
        return null;
    }

    private void GetMouseSelectedArea(out Vector3 lowerLeft, out Vector3 upperRight)
    {
        Vector3 currentMousePosition = Utility.GetMouseWorldPosition();
        lowerLeft = new Vector3(
            Mathf.Min(mouseDownPosition.x, currentMousePosition.x),
            Mathf.Min(mouseDownPosition.y, currentMousePosition.y)
            );
        upperRight = new Vector3(
            Mathf.Max(mouseDownPosition.x, currentMousePosition.x),
            Mathf.Max(mouseDownPosition.y, currentMousePosition.y)
            );
        lowerLeft = ObjectGrid.GetWorldPosition(lowerLeft, false);
        upperRight = ObjectGrid.GetTopRightCornerWorldPosition(upperRight);
    }

    private void PauseGame(bool state)
    {
        gamePaused = state;
        Time.timeScale = state ? 0 : 1;
    }

    private void ToggleUIState() => UI.SetActive(!UI.activeSelf);
}
