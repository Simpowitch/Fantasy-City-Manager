using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    ResourceObjectNetwork network;
    public ObjectTile ObjectTile { get; private set; }

    [SerializeField] CityResource.Type type = CityResource.Type.Stone;
    public CityResource.Type Type { get => type; }

    public Citizen workOccupiedBy;

    public bool markedForHarvest;

    //Called when the object is spawned in
    public void Spawned(ResourceObjectNetwork network, ObjectTile objectTile)
    {
        this.network = network;
        this.ObjectTile = objectTile;
        this.ObjectTile.ResourceObject = this;
    }

    //Called when harvested 
    public void Despawn()
    {
        network.RemoveObject(this);
        ObjectTile.ResourceObject = null;
        GameObject.Destroy(this.gameObject);
    }
}
