using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Grid Grid;
    public GameObject MapRoot;
    public Vector2Int mapHalfSize;
    public float CellSize;
    private Map<GameObject> levelMap;
    private Map<Vector2Int?> paths;
    private Vector2Int[] neighbours = { new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private Vector2Int[] neighboursReversed = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
    public Vector2Int BasePosition;
    void Start()
    {
        levelMap = new Map<GameObject>(mapHalfSize);
        paths = new Map<Vector2Int?>(mapHalfSize);
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
        GeneratePaths(BasePosition);
        return obj;
    }

    public Vector3 GetPath(Vector3 start)
    {
        var mapStart = WorldToMap(start);
        var mapGoal = paths.Get(mapStart.x, mapStart.y);
        return mapGoal.HasValue ? MapToWorld(mapGoal.Value) : start;
    }

    public void GeneratePaths(Vector2Int goal)
    {
        var frontier = new Queue<Vector2Int>();
        frontier.Enqueue(goal);
        var reached = new Map<bool>(mapHalfSize);
        reached.Set(goal, true);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var neighbours = GetNeighbours(current);
            foreach (var next in neighbours)
            {
                if (!reached.Get(next))
                {
                    frontier.Enqueue(next);
                    reached.Set(next, true);
                    paths.Set(next, current);
                }
            }
        }
        for (var x = -mapHalfSize.x; x < mapHalfSize.x; x++)
        {
            for (var y = -mapHalfSize.y; y < mapHalfSize.y; y++)
            {
                if (paths.Get(x, y).HasValue)
                {
                    var start = MapToWorld(new Vector2Int(x, y));
                    var target = MapToWorld(paths.Get(x, y).Value);
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
            if (levelMap.InBounds(neighbour.x, neighbour.y) &&
                levelMap.Get(neighbour.x, neighbour.y, false) == null)
            {
                result.Add(neighbour);
            }
        }
        return result;
    }

    private Vector2Int WorldToMap(Vector3 position)
    {
        return (Vector2Int)Grid.WorldToCell(position);
    }

    private Vector3 MapToWorld(Vector2Int position)
    {
        return Grid.GetCellCenterWorld((Vector3Int)position);
    }
}