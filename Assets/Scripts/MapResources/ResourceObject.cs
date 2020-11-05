using UnityEngine;

public class ResourceObject : MonoBehaviour, IViewable
{
    protected ResourceObjectNetwork network;
    public ObjectTile ObjectTile { get; private set; }

    [SerializeField] CityResource yieldOnHarvest = null;
    [SerializeField] SpriteRenderer markRenderer = null;
    public CityResource.Type Type { get => yieldOnHarvest.type; }

    [HideInInspector] public Citizen workOccupiedBy;
    public bool CanBeHarvested { get; protected set; } = true;
    public bool MarkedForHarvest { get; private set; }
    public enum HarvestMarkMode { Automatic, Manual}
    [SerializeField] HarvestMarkMode harvestMode = HarvestMarkMode.Manual;
    public HarvestMarkMode HarvestMode { get => harvestMode; }
    public InfoChangeHandler InfoChangeHandler { get; set; }
    [SerializeField] string objectName = "";
    public string Name { get => objectName; set => objectName = value; }
    public string ActionDescription => CanBeHarvested ? "Harvestable" : "Not Harvestable";

    //Called when the object is spawned in
    public virtual void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        this.network = network;
        this.ObjectTile = objectTile;
        this.ObjectTile.ResourceObject = this;
    }

    public virtual void StartHarvesting() { }

    public virtual CityResource Harvest()
    {
        Despawn();
        return yieldOnHarvest;
    }

    public void MarkForHarvest(bool state)
    {
        MarkedForHarvest = state;
        markRenderer.enabled = state;
    }

    //Called when harvested 
    protected virtual void Despawn()
    {
        network.RemoveStatic(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject, 0.01f);
    }

    public virtual string GetSpeciality() => "";

    public virtual float GetPrimaryStatValue() => 1;

    public virtual string GetPrimaryStatName() => "Harvestable";

    public Need[] GetNeeds() => null;

    public bool Interactable() => CanBeHarvested;

    // Player Interaction with the resource object - Returns true if possible
    public bool PlayerInteraction(PlayerCharacter playerCharacter, PlayerInput playerInput, PlayerTaskSystem playerTaskSystem)
    {
        if (!Interactable())
            return false;

        StartHarvesting();
        switch (Type)
        {
            case CityResource.Type.Wood:
                playerCharacter.MoveTo(transform.position, UnitAnimator.ActionAnimation.ChopWood, true);
                break;
            case CityResource.Type.Gold:
            case CityResource.Type.Stone:
            case CityResource.Type.Iron:
                playerCharacter.MoveTo(transform.position, UnitAnimator.ActionAnimation.Mine, true);
                break;
            case CityResource.Type.Food:
                playerCharacter.MoveTo(transform.position, UnitAnimator.ActionAnimation.Harvest, true);
                break;
        }

        //Start Task
        playerTaskSystem.StartTask(() =>
        {
            playerCharacter.City.cityStats.Inventory.Add(Harvest());
            playerCharacter.UnitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.Idle);
            playerInput.inputEnabled = true;
            playerCharacter.SetColliderState(true);
        });
        return true;
    }
}
