using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid
{
    public class Node
    {
        public int x, y;
        public bool isWalkable;
        public float gCost, hCost, fCost;
        public Vector3 worldPosition;
        public Node cameFromNode;

        public Node(int x, int y, bool isWalkable, Vector3 worldPos)
        {
            this.x = x;
            this.y = y;
            this.isWalkable = isWalkable;
            this.worldPosition = worldPos;
            ResetCosts();
        }

        public void ResetCosts()
        {
            gCost = float.MaxValue;
            hCost = 0;
            fCost = 0;
            cameFromNode = null;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }

    private Node[,] nodeGrid;

    public PathfindingGrid(int width, int height, int[,] gridArray, Vector3[,] worldPos)
    {
        nodeGrid = new Node[width, height];
        InitializeNodes(gridArray, worldPos);
    }

    //Initializes nodes based on grid.
    public void InitializeNodes(int[,] gridsArray, Vector3[,] worldPos)
    {
        for (int x = 0; x < nodeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < nodeGrid.GetLength(1); y++)
            {
                //Set walkability and world positions.
                bool isWalkable = gridsArray[x, y] < 1;
                Vector3 worldPosition = worldPos[x, y];
                nodeGrid[x, y] = new Node(x, y, isWalkable, worldPosition);  //Update walkability.
            }
        }
        //DebugNodeGrid();
        //DebugNodeGridPositions();
    }

    //Return path.
    public List<Vector3> FindPath(int startX, int startY, int targetX, int targetY)
    {
        Node startNode = GetNode(startX, startY);
        Node targetNode = GetNode(targetX, targetY);
        //Debug
        //Debug.Log($"Start Node: {startNode?.worldPosition}, Walkable: {startNode?.isWalkable}");
        //Debug.Log($"Target Node: {targetNode?.worldPosition}, Walkable: {targetNode?.isWalkable}");

        if (startNode == null || targetNode == null || !targetNode.isWalkable)
        {
            Debug.LogWarning("Invalid start or target node, or target node is not walkable.");
            return null;
        }

        // A* Pathfinding algorithm
        List<Node> openList = new List<Node> { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, targetNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == targetNode)
            {
                //Path found!
                return CalculatePath(targetNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || !neighbor.isWalkable)
                {
                    continue;
                }

                float tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbor);

                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.cameFromNode = currentNode;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistance(neighbor, targetNode);
                    neighbor.CalculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        //No path found.
        return null;
    }

    //Get node at grid position.
    public Node GetNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < nodeGrid.GetLength(0) && y < nodeGrid.GetLength(1))
        {
            return nodeGrid[x, y];
        }
        return null;
    }

    //Get neighbors of a node.
    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;  //Skip the node itself.

                int checkX = node.x + x;
                int checkY = node.y + y;

                Node neighbor = GetNode(checkX, checkY);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    //Get the node with the lowest fCost.
    private Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];

        foreach (Node node in openList)
        {
            if (node.fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = node;
            }
        }

        return lowestFCostNode;
    }

    //Calculate distance between two nodes (Euclidean Distance for grid).
    private float CalculateDistance(Node a, Node b)
    {
        return Vector3.Distance(a.worldPosition, b.worldPosition);
    }

    //Calculate with world pos.
    private List<Vector3> CalculatePath(Node targetNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = targetNode;

        while (currentNode != null)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();  //Reverse the path to go from start to target.
        return path;
    }
    #region Debug
    public void DebugNodeGrid()
    {
        Debug.Log("------ Node Grid Debug ------");
        for (int x = 0; x < nodeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < nodeGrid.GetLength(1); y++)
            {
                Node node = nodeGrid[x, y];
                if (!node.isWalkable)
                    Debug.Log($"Node ({node.x},{node.y}) - Walkable: {node.isWalkable}");
            }
        }
        Debug.Log("----------------------------");
    }
    public void DebugNodeGridPositions()
    {
        Debug.Log("------ Node Grid Debug Positions------");
        for (int x = 0; x < nodeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < nodeGrid.GetLength(1); y++)
            {
                Node node = nodeGrid[x, y];
                Debug.Log($"Node ({node.x},{node.y}) - Position: {node.worldPosition}");
            }
        }
        Debug.Log("----------------------------");
    }
    #endregion
}
