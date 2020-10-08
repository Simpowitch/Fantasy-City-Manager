using System.Collections.Generic;
using UnityEngine;

public class PathSeeker : MonoBehaviour
{
    public delegate void PathHandler(PathNode nextNode);
    public event PathHandler OnPathUpdated;

    float distanceToAcceptAsArrived = 0.1f;
    float pathLostDistance = 2f;

    public City City { get; set; }

    public List<PathNode> Path
    {
        get;
        private set;
    }

    public void FindPathTo(Vector3 target)
    {
        Path = City.Pathfinding.FindPath(this.transform.position, target);
        OnPathUpdated?.Invoke(Path != null && Path.Count > 0 ? Path[0] : null);
    }

    public bool HasPath
    {
        get => Path != null && Path.Count > 0;
    }

    public bool CheckIfNodeArrived()
    {
        if (HasPath)
        {
            if (Vector3.Distance(Path[0].WorldPosition, this.transform.position) <= distanceToAcceptAsArrived)
            {
                Path.RemoveAt(0);
                OnPathUpdated?.Invoke(Path.Count > 0 ? Path[0] : null);
                return true;
            }
        }
        return false;
    }

    public bool IsLost()
    {
        if (HasPath)
        {
            if (Vector3.Distance(Path[0].WorldPosition, this.transform.position) <= pathLostDistance)
            {
                return true;
            }
        }
        return false;
    }
}
