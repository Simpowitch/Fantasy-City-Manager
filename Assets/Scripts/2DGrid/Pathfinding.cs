using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{
    const int MOVE_STRAIGHT_COST = 10, MOVE_DIAGONAL_COST = 14;

    public readonly Grid<PathNode> grid;
    List<PathNode> openList;
    List<PathNode> closedList;

    public Pathfinding(int width, int height, float cellSize, Vector3 offsetPosition)
    {
        grid = new Grid<PathNode>(width, height, cellSize, offsetPosition, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public List<PathNode> FindPath(Vector2 fromPosition, Vector2 toPosition)
    {
        grid.GetXY(fromPosition, out int x1, out int y1);
        grid.GetXY(toPosition, out int x2, out int y2);
        return FindPath(x1, y1, x2, y2);
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            return null;
        }

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        //Reset all nodes
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode) + neighbourNode.MovementPenalty;
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        //Out of nodes on the open list
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Left down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //Left up
            if (currentNode.y + 1 < grid.Height) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x +1 < grid.Width)
        {
            //Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //Right down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Right up
            if (currentNode.y + 1 < grid.Height) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        //Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if (currentNode.y + 1 < grid.Height) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y) => grid.GetGridObject(x, y);

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
