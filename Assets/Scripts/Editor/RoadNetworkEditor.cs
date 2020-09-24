using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadNetwork))]
public class RoadNetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoadNetwork roadNetwork = (RoadNetwork)target;

        //string boolLabel = roadNetwork.PathfindingPenaltiesShown ? "Hide" : "Show";
        if (GUILayout.Button("Toggle Show Pathfinding Penalty"))
        {
            roadNetwork.ShowPathfindingPenalties(!roadNetwork.PathfindingPenaltiesShown);
        }
    }
}
