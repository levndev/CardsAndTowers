using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map<T>
{
    private Vector2Int halfSize;
    private readonly T[,] map;
    public Map(Vector2Int HalfSize)
    {
        halfSize = HalfSize;
        map = new T[halfSize.x * 2, halfSize.y * 2];
    }

    public T Get(Vector2Int index, bool safe = true)
    {
        return Get(index.x, index.y, safe);
    }

    public T Get(int x, int y, bool safe = true)
    {
        if (safe && !InBounds(x, y))
            throw new System.IndexOutOfRangeException();
        x += halfSize.x;
        y += halfSize.y;
        return map[x, y];
    }

    public void Set(Vector2Int index, T value, bool safe = true)
    {
        Set(index.x, index.y, value, safe);
    }

    public void Set(int x, int y, T value, bool safe = true)
    {
        if (safe && !InBounds(x, y))
            throw new System.IndexOutOfRangeException();
        x += halfSize.x;
        y += halfSize.y;
        map[x, y] = value;
    }

    public bool InBounds(Vector2Int index)
    {
        return InBounds(index.x, index.y);
    }

    public bool InBounds(int x, int y)
    {
        x += halfSize.x;
        y += halfSize.y;
        return x >= 0 &&
                y >= 0 &&
                x < halfSize.x * 2 &&
                y < halfSize.y * 2;
    }
}
