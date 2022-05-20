using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Grid Grid;
    public GameObject MapRoot;
    public Vector2Int mapHalfSize;
    public float CellSize;
    private GameObject[,] map;
    private Vector2Int?[,] paths;
    private Vector2Int[] neighbours = { new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private Vector2Int[] neighboursReversed = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
    public Vector2Int BasePosition;
    void Start()
    {
        map = new GameObject[mapHalfSize.x * 2, mapHalfSize.y * 2];
        paths = new Vector2Int?[mapHalfSize.x * 2, mapHalfSize.y * 2];
        GeneratePaths(BasePosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InstantiateObject(GameObject original, Vector3 position)
    {
        //position += new Vector3(-0.5f, 0.5f, 0);
        var size = original.GetComponent<SizeData>().Size;
        var mapPosition = WorldToMap(position);
        //Debug.Log(gridPosition);
        for (var x = mapPosition.x; x < mapPosition.x + size.x; x++)
        {
            for (var y = mapPosition.y; y < mapPosition.y + size.y; y++)
            {
                if (map[x, y] != null)
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
                map[x, y] = obj;
            }
        }
        GeneratePaths(BasePosition);
        return obj;
    }

    public Vector3 GetPath(Vector3 start)
    {
        var mapStart = WorldToMap(start);
        var mapGoal = paths[mapStart.x, mapStart.y];
        return mapGoal.HasValue ? MapToWorld(mapGoal.Value) : start;
    }

    public void GeneratePaths(Vector2Int goal)
    {
        if (goal.x < 0 || goal.y < 0 || goal.x > map.GetLength(0) || goal.y > map.GetLength(1))
            throw new System.IndexOutOfRangeException();
        var frontier = new Queue<Vector2Int>();
        frontier.Enqueue(goal);
        var reached = new bool[mapHalfSize.x * 2, mapHalfSize.y * 2];
        reached[goal.x, goal.y] = true;
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var neighbours = GetNeighbours(current);
            foreach (var next in neighbours)
            {
                if (!reached[next.x, next.y])
                {
                    frontier.Enqueue(next);
                    reached[next.x, next.y] = true;
                    paths[next.x, next.y] = current;
                }
            }
        }
        for (var x = 0; x < mapHalfSize.x * 2; x++)
        {
            for (var y = 0; y < mapHalfSize.y * 2; y++)
            {
                if (paths[x, y].HasValue)
                {
                    var start = MapToWorld(new Vector2Int(x, y));
                    var target = MapToWorld(paths[x, y].Value);
                    Debug.DrawLine(start, target, Color.red, 50f);
                }
            }
        }
    }

    private List<Vector2Int> GetNeighbours(Vector2Int position)
    {
        var result = new List<Vector2Int>();
        var id = (position.x + position.y) % 2;
        for(var i = 0; i < 4; i++)
        {
            Vector2Int neighbour;
            if (id == 0)
            {
                neighbour = position + neighbours[i];
            }
            else
            {
                neighbour = position + neighboursReversed[i];
            }
            if (neighbour.x >= 0 &&
                neighbour.y >= 0 &&
                neighbour.x < mapHalfSize.x * 2 &&
                neighbour.y < mapHalfSize.y * 2 &&
                map[neighbour.x, neighbour.y] == null)
            {
                result.Add(neighbour);
            }
        }
        return result;
    }

    private Vector2Int WorldToMap(Vector3 position)
    {
        return mapHalfSize + (Vector2Int)Grid.WorldToCell(position);
    }

    private Vector3 MapToWorld(Vector2Int position)
    {
        return Grid.GetCellCenterWorld((Vector3Int)(position - mapHalfSize));
    }
}
