using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [Header("Static references")]
    [SerializeField] Player player = null;
    [SerializeField] ConstructionSystem constructionSystem = null;
    [SerializeField] MouseTooltip mouseTooltip = null;
    [SerializeField] PlayerTaskSystem playerTaskSystem = null;
    City city;
    Grid<ObjectTile> ObjectGrid { get => city.ObjectGrid; }
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
    public enum MouseMode { PlayerInteraction, BuildRoad, BuildStructure, RemoveRoad, RemoveStructure, Harvest, CancelHarvest, MAX = 6 }
    MouseMode mode;
    MouseMode Mode
    {
        get => mode;
        set
        {
            mode = value;
            AllowedConstruction = true;
            constructionSystem.ClearPreviews();
            canvasGrid.SetGridOutlines(value != MouseMode.PlayerInteraction);
            Color modeObjectColor = modeObjectRenderer.color;
            switch (value)
            {
                case MouseMode.PlayerInteraction:
                    canMultiSelect = false;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = normal;
                    mouseTooltip.Hide();
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case MouseMode.BuildRoad:
                    canMultiSelect = true;
                    modeObjectRenderer.sprite = build;
                    modeObjectColor.a = 0.5f;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Road;
                    break;
                case MouseMode.BuildStructure:
                    canMultiSelect = false;
                    modeObjectRenderer.sprite = build;
                    modeObjectColor.a = 0.5f;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Structure;
                    break;
                case MouseMode.RemoveRoad:
                    canMultiSelect = true;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = remove;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case MouseMode.RemoveStructure:
                    canMultiSelect = false;
                    modeObjectColor.a = 1;
                    modeObjectRenderer.sprite = remove;
                    constructionSystem.ConstructionMode = ConstructionSystem.Mode.Off;
                    break;
                case MouseMode.Harvest:
                case MouseMode.CancelHarvest:
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
            Mode = MouseMode.PlayerInteraction;

        if (Input.GetKeyDown(KeyCode.R))
            constructionSystem.ChangeFacingRotationDirection();

        if (Input.GetKeyDown(KeyCode.P))
            PauseGame(!gamePaused);
        if (Input.GetKeyDown(KeyCode.U))
            ToggleUIState();


        //Player character movement
        Vector3 aim = playerCharacter.transform.position;

        if (Input.GetKey(KeyCode.W)) //Up
            aim += new Vector3(0, 1);
        if (Input.GetKey(KeyCode.D)) //Right
            aim += new Vector3(1, 0);
        if (Input.GetKey(KeyCode.S)) //Down
            aim += new Vector3(0, -1);
        if (Input.GetKey(KeyCode.A)) //Left
            aim += new Vector3(-1, 0);

        playerCharacter.SetAim(aim);
    }

    void Hoover()
    {
        Vector3 centeredWorldPosition = ObjectGrid.GetWorldPosition(Utility.GetMouseWorldPosition(), true);

        selectionObject.transform.position = centeredWorldPosition;
        switch (Mode)
        {
            case MouseMode.PlayerInteraction:
                break;
            case MouseMode.BuildRoad:
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
            case MouseMode.RemoveRoad:
                break;
            case MouseMode.BuildStructure:
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
            case MouseMode.RemoveStructure:
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
            case MouseMode.PlayerInteraction:
                ObjectTile tileUnderMouse = ObjectGrid.GetGridObject(worldPosition);
                if (tileUnderMouse.ResourceObject != null)
                    InteractWithResourceObject(tileUnderMouse.ResourceObject);
                break;
            case MouseMode.BuildRoad:
                if (allowedConstruction)
                {
                    foreach (var objectTile in selectedObjectTiles)
                    {
                        constructionSystem.BuildRoad(objectTile.CenteredWorldPosition);
                    }
                }
                break;
            case MouseMode.RemoveRoad:
                if (allowedConstruction)
                {
                    foreach (var objectTile in selectedObjectTiles)
                    {
                        constructionSystem.RemoveRoad(objectTile.CenteredWorldPosition);
                    }
                }
                break;
            case MouseMode.BuildStructure:
                if (allowedConstruction)
                {
                    constructionSystem.BuildStructure(worldPosition);
                }
                break;
            case MouseMode.RemoveStructure:
                if (allowedConstruction)
                {
                    constructionSystem.RemoveStructure(worldPosition);
                }
                break;
            case MouseMode.Harvest:
                foreach (var objectTile in selectedObjectTiles)
                {
                    ResourceObject resourceObject = objectTile.ResourceObject;
                    if (resourceObject != null && resourceObject.CanBeHarvested && resourceObject.HarvestMode == ResourceObject.HarvestMarkMode.Manual)
                        resourceObject.MarkForHarvest(true);
                }
                break;
            case MouseMode.CancelHarvest:
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
            case MouseMode.PlayerInteraction:
                break;
            case MouseMode.BuildRoad:
            case MouseMode.RemoveRoad:
            case MouseMode.BuildStructure:
            case MouseMode.RemoveStructure:
                constructionSystem.ClearPreviews();
                break;
        }
    }

    public void SetMode(int intMode)
    {
        if (intMode < 0 || intMode > (int)MouseMode.MAX)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        Mode = (MouseMode)intMode;
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

    private void InteractWithResourceObject(ResourceObject resource)
    {
        switch (resource.Type)
        {
            case CityResource.Type.Gold:
                break;
            case CityResource.Type.Wood:
                if (resource.CanBeHarvested)
                {
                    resource.StartHarvesting();
                    playerCharacter.UnitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.ChopWood);

                    //Start Task
                    playerTaskSystem.StartTask(() =>
                    {
                        city.cityStats.Inventory.Add(resource.Harvest());
                        playerCharacter.UnitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.Idle);
                        });
                }
                break;
            case CityResource.Type.Stone:
                break;
            case CityResource.Type.Iron:
                break;
            case CityResource.Type.Food:
                break;
        }
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
