using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Grid Grid;
    public GameObject MapRoot;
    public GameObject[,] map;
    public Vector2Int mapHalfSize;
    public float CellSize;
    void Start()
    {
        map = new GameObject[mapHalfSize.x * 2, mapHalfSize.y * 2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InstantiateObject(GameObject original, Vector3 position)
    {
        //position += new Vector3(-0.5f, 0.5f, 0);
        var size = original.GetComponent<SizeData>().Size;
        var gridPosition = mapHalfSize + (Vector2Int)Grid.WorldToCell(position);
        //Debug.Log(gridPosition);
        for (var x = gridPosition.x; x < gridPosition.x + size.x; x++)
        {
            for (var y = gridPosition.y; y < gridPosition.y + size.y; y++)
            {
                if (map[x, y] != null)
                {
                    return null;
                }
            }
        }
        var worldPosition = Grid.GetCellCenterWorld(Grid.WorldToCell(position)) + new Vector3(CellSize / 2.0f, CellSize / 2.0f, 0);
        var obj = Instantiate(original, worldPosition, new Quaternion(), MapRoot.transform);
        for (var x = gridPosition.x; x < gridPosition.x + size.x; x++)
        {
            for (var y = gridPosition.y; y < gridPosition.y + size.y; y++)
            {
                map[x, y] = obj;
            }
        }
        return obj;
    }
}
