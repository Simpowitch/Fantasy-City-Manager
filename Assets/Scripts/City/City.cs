using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public float CellSize { get; } = 1;

    [Header("Static references")]
    [SerializeField] Transform unitTransformParent = null;
    [SerializeField] Visitor visitorPrefab = null;
    [SerializeField] Citizen citizenPrefab = null;
    [SerializeField] CanvasGrid canvasGrid = null;

    [Header("Game references - Adaptable")]
    public CityHall cityHall = null;
    public List<Residence> residentialBuildings = null;
    public List<Commercial> commercialBuidlings = null;
    public List<Tavern> taverns = null;
    public List<Workplace> workplaces = null;
    public List<CityGate> cityGates = null;
    public List<Farmland> farmlands = null;
    public Transform[] cityEntrances = null;
    public List<Structure> unfinishedStructures = null;

    public List<INeedProvider> HungerProviders { get; private set; } = new List<INeedProvider>();
    public List<INeedProvider> EnergyProviders { get; private set; } = new List<INeedProvider>();
    public List<INeedProvider> RecreationProviders { get; private set; } = new List<INeedProvider>();
    public List<INeedProvider> SocialProviders { get; private set; } = new List<INeedProvider>();

    [Header("Debug")]
    public int minVisits = 1;
    public int maxVisits = 20;

    public List<BuilderEmployment> BuilderEmployments = new List<BuilderEmployment>();

    [Header("Incomes")]
    public int cityGateToll = 1;
    public int residentialDailyTax = 1;
    public int workplacesDailyTax = 2;

    [Header("Expenses")]
    public int builderFee = 1;

    public Pathfinding Pathfinding { get; private set; }
    public Grid<ObjectTile> ObjectGrid { get; private set; }
    public RoadNetwork RoadNetwork { get; private set; }
    public ResourceObjectNetwork ResourceObjectNetwork { get; private set; }

    public CityStats cityStats = new CityStats();

    private void OnEnable()
    {
        DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
    }

    private void OnDisable()
    {
        DayNightSystem.OnPartOfTheDayChanged -= PartOfDayChange;
    }

    public void Setup(int xSize, int ySize, int startGold, int startWood, int startStone, int startIron, int startFood, int startCitizens, Transform player)
    {
        cityStats.Setup(startGold, startWood, startStone, startIron, startFood);

        RoadNetwork = GetComponent<RoadNetwork>();
        ResourceObjectNetwork = GetComponent<ResourceObjectNetwork>();
        Pathfinding = new Pathfinding(xSize, ySize, CellSize, Vector3.zero);
        ObjectGrid = new Grid<ObjectTile>(xSize, ySize, CellSize, Vector3.zero, (Grid<ObjectTile> g, int x, int y) => new ObjectTile(g, x, y));
        RoadNetwork.SetUp(ObjectGrid, Pathfinding.grid);
        ResourceObjectNetwork.Setup(ObjectGrid);
        canvasGrid.Setup(ObjectGrid, Pathfinding.grid);

        //Pre-placed structures
        foreach (var residence in residentialBuildings)
        {
            ConfirmBuildingPlacements(residence);
        }
        foreach (var workplace in workplaces)
        {
            ConfirmBuildingPlacements(workplace);
        }
        foreach (var tavern in taverns)
        {
            ConfirmBuildingPlacements(tavern);
        }
        foreach (var commercialBuidling in commercialBuidlings)
        {
            ConfirmBuildingPlacements(commercialBuidling);
        }
        foreach (var cityGate in cityGates)
        {
            ConfirmBuildingPlacements(cityGate);
        }

        //Spawn start citizens
        Vector3[] positions = GridUtilities.GetPositionsAround(player.position, startCitizens / 2, startCitizens);
        for (int i = 0; i < startCitizens; i++)
        {
            SpawnCitizen(positions[i]);
        }
    }

    void ConfirmBuildingPlacements(Structure structure)
    {
        structure.AnchorPoint = structure.transform.position;
        structure.Load(this);
    }
    public void AddConstructionArea(Structure unfinishedStructure)
    {
        unfinishedStructures.Add(unfinishedStructure);
        UpdateBuildersWorkingAreas();
    }

    public void RemoveConstructionArea(Structure constructedStructure)
    {
        unfinishedStructures.Remove(constructedStructure);
        UpdateBuildersWorkingAreas();
    }

    private void UpdateBuildersWorkingAreas()
    {
        foreach (var builder in BuilderEmployments)
        {
            builder.unfinishedStructures = unfinishedStructures;
        }
    }

    void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay)
    {
        switch (partOfDay)
        {
            case DayNightSystem.PartOfTheDay.Night:
                break;
            case DayNightSystem.PartOfTheDay.Morning:
                break;
            case DayNightSystem.PartOfTheDay.Day:
                SpawnVisitors(minVisits, maxVisits); //TODO CHANGE TO NUMBERS BASED ON CITY POP OR SIMILAR
                break;
            case DayNightSystem.PartOfTheDay.Evening:
                SimulateBudget();
                break;
        }
    }

    void SpawnVisitors(int minVisitors, int maxVisitors)
    {
        int rng = Random.Range(minVisitors, maxVisitors);
        for (int i = 0; i < rng; i++)
        {
            //Spawn visitor
            Visitor visitor = Instantiate(visitorPrefab);
            visitor.transform.SetParent(unitTransformParent);
            Transform entrance = Utility.ReturnRandom(cityEntrances);
            visitor.transform.position = entrance.position;
            visitor.Setup(this);
        }
    }

    
    void SpawnCitizen(Vector3 position)
    {
        Citizen citizen = Instantiate(citizenPrefab);
        citizen.transform.SetParent(unitTransformParent);
        citizen.transform.position = position;
        citizen.Setup(this);
    }

    #region Residence
    public bool HasUnFilledResidences() => UnfilledResidences() > 0;
    public int UnfilledResidences()
    {
        int number = 0;
        foreach (var residence in residentialBuildings)
        {
            number += residence.NumberOfUnfilledResidenceSpots;
        }
        return number;
    }
    public Residence GetRandomFreeHome() => Utility.ReturnRandomElementWithCondition(residentialBuildings, (residence) => residence.CanMoveIn());
    #endregion
    #region Workplaces
    public bool UnFilledEmployments(out List<Employment> unfilledEmployments)
    {
        unfilledEmployments = new List<Employment>();
        foreach (var workplace in workplaces)
        {
            unfilledEmployments.AddRange(workplace.UnfilledPositions);
        }
        foreach (var builderEmployment in BuilderEmployments)
        {
            if (!builderEmployment.PositionFilled)
            {
                unfilledEmployments.Add(builderEmployment);
            }
        }
        return unfilledEmployments.Count > 0;
    }
    #endregion

    void SimulateBudget()
    {
        int change = 0;

        //Incomes
        foreach (var residence in residentialBuildings)
        {
            if (residence.IsFunctional())
                change += residentialDailyTax;
        }
        foreach (var workplace in workplaces)
        {
            if (workplace.IsFunctional())
                change += workplacesDailyTax;
        }

        //Expenses
        foreach (var builder in BuilderEmployments)
        {
            if (builder.PositionFilled)
                change -= builderFee;
        }

        cityStats.Inventory.Add(new CityResource(CityResource.Type.Gold, change));
    }

    public Task CreateLeaveCityTask(Unit unit)
    {
        ActionTimer onTaskEnd = new ActionTimer(0.1f, () => Destroy(unit.gameObject), false);
        Task leaveTask = new Task("Leaving the city", ThoughtFileReader.GetText(unit.UnitPersonality, "leaveCity"), onTaskEnd, Utility.ReturnRandom(cityEntrances).position, UnitAnimator.ActionAnimation.Idle, false);
        return leaveTask;
    }

    //private void OnDrawGizmos()
    //{
    //    if (ObjectGrid != null)
    //    {
    //        foreach (ObjectTile tile in ObjectGrid.gridArray)
    //        {
    //            tile.PrintStateToCanvasObject();
    //        }
    //    }
    //}
}