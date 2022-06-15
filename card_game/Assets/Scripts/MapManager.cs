using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

public class MapManager : MonoBehaviour
{
    public Grid Grid;
    public GameObject MapRoot;
    public Vector2Int mapHalfSize;
    private Array2D<GameObject> levelMap;
    private Array2D<Vector2Int?> paths;
    private Vector2Int[] neighbours = { new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0),
                                        new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1) };
    private Vector2Int[] neighboursReversed = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
                                                new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1) };
    public Vector2Int BasePosition;
    
    void Start()
    {
        levelMap = new Array2D<GameObject>(mapHalfSize);
        paths = new Array2D<Vector2Int?>(mapHalfSize);   
        for (var i = 0; i < MapRoot.transform.childCount; i++)
        {
            var child = MapRoot.transform.GetChild(i);
            var size = child.GetComponent<SizeData>().Size;
            var mapPosition = WorldToMap(child.transform.position - new Vector3(size.x / 2.0f - 0.5f, size.y / 2.0f - 0.5f, 0));
            for (var x = mapPosition.x; x < mapPosition.x + size.x; x++)
            {
                for (var y = mapPosition.y; y < mapPosition.y + size.y; y++)
                {
                    levelMap.Set(x, y, child.gameObject);
                }
            }
        }
        GeneratePaths(BasePosition);
    }

    public GameObject InstantiateObject(GameObject original, Vector3 position, bool regeneratePaths = true)
    {
        //position += new Vector3(-0.5f, 0.5f, 0);
        var size = original.GetComponent<SizeData>().Size;
        var mapPosition = GetBuildingAnchor(position, size);
        //Debug.Log(gridPosition);
        for (var x = mapPosition.x; x < mapPosition.x + size.x; x++)
        {
            for (var y = mapPosition.y; y < mapPosition.y + size.y; y++)
            {
                if (levelMap.Get(x, y) != null)
                {
                    return null;
                }
            }
        }
        var worldPosition = MapToWorld(mapPosition);
        worldPosition += new Vector3(size.x / 2.0f - 0.5f, size.y / 2.0f - 0.5f, 0);
        var obj = Instantiate(original, worldPosition, new Quaternion(), MapRoot.transform);
        for (var x = mapPosition.x; x < mapPosition.x + size.x; x++)
        {
            for (var y = mapPosition.y; y < mapPosition.y + size.y; y++)
            {
                levelMap.Set(x, y, obj);
            }
        }
        if (regeneratePaths)
            RegeneratePaths();
        return obj;
    }

    public void RegeneratePaths()
    {
        GeneratePaths(BasePosition);
    }

    public Stack<Vector3> GetPath(Vector3 start, Vector3 goal)
    {
        var mapStart = WorldToMap(start);
        var mapGoal = WorldToMap(goal);
        return AStar(mapStart, mapGoal);
    }

    public Vector3 GetPathToBase(Vector3 start)
    {
        var mapStart = WorldToMap(start);
        var mapGoal = paths.Get(mapStart.x, mapStart.y);
        return mapGoal.HasValue ? MapToWorld(mapGoal.Value) : start;
    }

    public void GeneratePaths(Vector2Int goal)
    {
        Dijkstra(goal);
        //SmoothePaths(goal);
        for (var x = -mapHalfSize.x; x < mapHalfSize.x; x++)
        {
            for (var y = -mapHalfSize.y; y < mapHalfSize.y; y++)
            {
                if (paths.Get(x, y).HasValue)
                {
                    var start = MapToWorld(new Vector2Int(x, y));
                    var target = MapToWorld(paths.Get(x, y).Value);
                    Debug.DrawLine(start, target, Color.red, 10f);
                }
            }
        }
    }

    private Stack<Vector3> AStar(Vector2Int start, Vector2Int goal)
    {
        var frontier = new SimplePriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);
        var costSoFar = new Array2D<int?>(mapHalfSize);
        var came_from = new Array2D<Vector2Int?>(mapHalfSize);
        costSoFar.Set(start, 0);
        came_from.Set(start, null);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == goal)
            {
                break;
            }
            var currentCost = costSoFar.Get(current).Value;
            var neighbours = GetNeighbours(current);
            foreach (var (next, cost) in neighbours)
            {
                var newCost = currentCost + cost;
                if (costSoFar.Get(next, false) == null || newCost < costSoFar.Get(next, false).Value)
                {
                    var priority = newCost + heuristic(next, goal);
                    costSoFar.Set(next, newCost, false);
                    frontier.Enqueue(next, priority);
                    came_from.Set(next, current);
                }
            }
        }
        {
            var path = new Stack<Vector3>();
            Vector2Int? next = goal;
            while(next.HasValue)
            {
                path.Push(MapToWorld(next.Value));
                next = came_from.Get(next.Value);
            }
            return path;
        }
        static int heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    private void Dijkstra(Vector2Int goal)
    {
        var frontier = new SimplePriorityQueue<Vector2Int>();
        frontier.Enqueue(goal, 0);
        var costSoFar = new Array2D<int?>(mapHalfSize);
        costSoFar.Set(goal, 0);
        paths.Set(goal, null);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var currentCost = costSoFar.Get(current).Value;
            var neighbours = GetNeighbours(current);
            foreach (var (next, cost) in neighbours)
            {
                var newCost = currentCost + cost;
                if (costSoFar.Get(next, false) == null || newCost < costSoFar.Get(next, false).Value)
                {
                    costSoFar.Set(next, newCost, false);
                    frontier.Enqueue(next, newCost);
                    paths.Set(next, current);
                }
            }
        }
    }

    private void Bfs(Vector2Int goal)
    {
        paths = new Array2D<Vector2Int?>(mapHalfSize);
        var frontier = new Queue<Vector2Int>();
        frontier.Enqueue(goal);
        var reached = new Array2D<bool>(mapHalfSize);
        reached.Set(goal, true);
        paths.Set(goal, null);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var neighbours = GetNeighbours(current);
            foreach (var (next, cost) in neighbours)
            {
                if (cost == 1 && !reached.Get(next))
                {
                    frontier.Enqueue(next);
                    reached.Set(next, true);
                    paths.Set(next, current);
                }
            }
        }
    }

    private void SmoothePaths(Vector2Int goal)
    {
        var goalWorld = MapToWorld(goal);
        for (var x = -mapHalfSize.x; x < mapHalfSize.x; x++)
        {
            for (var y = -mapHalfSize.y; y < mapHalfSize.y; y++)
            {
                var node = new Vector2Int(x, y);
                var nodeWorld = MapToWorld(node);
                if (levelMap.Get(node, false) == null)
                {
                    var heading = goalWorld - nodeWorld;
                    var distance = heading.magnitude;
                    var direction = heading / distance;
                    var hit = Physics2D.Raycast(node, direction, distance, LayerMask.GetMask("Towers", "Walls"));
                    if (hit.collider != null)
                    {
                        continue;
                    }
                    else
                    {
                        paths.Set(node, goal);
                    }
                }
            }
        }
    }

    private List<(Vector2Int, int)> GetNeighbours(Vector2Int position)
    {

        var result = new List<(Vector2Int, int)>();
        var id = (position.x + position.y) % 2;
        for (var i = 0; i < neighbours.Length; i++)
        {
            Vector2Int neighbour;
            neighbour = position + neighbours[i];
            if (levelMap.InBounds(neighbour))
            {
                var obj = levelMap.Get(neighbour);
                var cost = obj == null ? 1 : 10;
                if (obj != null)
                {
                    if (obj.tag == "Indestructible")
                    {
                        continue;
                    }
                }
                result.Add((neighbour, cost));
            }
        }
        return result;
    }

    public Vector2Int GetBuildingAnchor(Vector3 position, Vector2Int size)
    {
        return WorldToMap(position - new Vector3(size.x / 2f - 0.5f, size.y / 2f - 0.5f, 0));
    }

    public Vector3 GetBuildingCenter(Vector3 position, Vector2Int size)
    {
        return MapToWorld(WorldToMap(position)) + new Vector3(size.x / 2f - 0.5f, size.y / 2f - 0.5f, 0);
    }

    public bool IsValidPlacement(Vector3 position, Vector2Int size)
    {
        var mapPosition = WorldToMap(position);
        for (var x = mapPosition.x; x < mapPosition.x + size.x; x++)
        {
            for (var y = mapPosition.y; y < mapPosition.y + size.y; y++)
            {
                if (levelMap.Get(x, y) != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector2Int WorldToMap(Vector3 position)
    {
        return (Vector2Int)Grid.WorldToCell(position);
    }

    public Vector3 MapToWorld(Vector2Int position)
    {
        return Grid.GetCellCenterWorld((Vector3Int)position);
    }
}
