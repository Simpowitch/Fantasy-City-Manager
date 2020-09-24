using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public bool markedForChopping = false;

    public ObjectTile ObjectTile { get; private set; }
    public PathNode PathNode { get; private set; }

    public void Spawned(ObjectTile objectTile, PathNode pathNode)
    {
        ObjectTile = objectTile;
        ObjectTile.Tree = this;
        PathNode = pathNode;
        PathNode.ChangeIsWalkable(false);
    }

    public void Despawn()
    {
        ObjectTile.Tree = null;
        PathNode.ChangeIsWalkable(true);
        GameObject.Destroy(this.gameObject);
    }
}
