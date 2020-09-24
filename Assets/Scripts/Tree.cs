using UnityEngine;

public class Tree : MonoBehaviour
{
    public bool markedForChopping = false;
    TreeNetwork treeNetwork;
    public ObjectTile ObjectTile { get; private set; }

    public Citizen workOccupiedBy;

    public void Spawned(TreeNetwork treeNetwork, ObjectTile objectTile)
    {
        this.treeNetwork = treeNetwork;
        ObjectTile = objectTile;
        ObjectTile.Tree = this;
    }

    public void Despawn()
    {
        treeNetwork.RemoveTree(this);
        ObjectTile.Tree = null;
        GameObject.Destroy(this.gameObject);
    }
}
