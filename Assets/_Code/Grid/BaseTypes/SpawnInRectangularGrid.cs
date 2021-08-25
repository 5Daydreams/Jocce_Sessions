using System;
using _Code.Grid;
using _Code.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class SpawnInRectangularGrid<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Grid Parameters")]
    [SerializeField] protected Vector3Int _tileCount = Vector3Int.one;
    [SerializeField] protected Vector3 _cellSpacing = Vector3.one;
    [SerializeField] protected FloatArrayValue _gridBoundaries;
    [SerializeField] protected FloatArrayValue _cellSpacingValues;
    protected T[,] _gridElements = new T[1,1];

    public abstract void SpawnGrid();

    public virtual void SetGridSize(int x, int y)
    {
        _tileCount.x = x;
        _tileCount.y = y;
        
        _cellSpacingValues.Value = new float[2];
        _cellSpacingValues.Value[0] = _cellSpacing.x;
        _cellSpacingValues.Value[1] = _cellSpacing.y;
    }

    public virtual void SetGridSize(Vector2Int vector)
    {
        SetGridSize(vector.x, vector.y);
    }

    public virtual T GetGridElement(int x, int y)
    {
        return _gridElements[x, y];
    }

    public virtual T GetGridElement(Vector2Int vector)
    {
        return GetGridElement(vector.x, vector.y);
    }

    public virtual void DeleteCurrentGrid()
    {
        for (int i = 0; i < _gridElements.GetLength(0); i++)
        {
            for (int j = 0; j < _gridElements.GetLength(1); j++)
            {
                Destroy(_gridElements[i,j]?.gameObject);
            }
        }
    }

    public virtual T GetRandomGridElement()
    {
        int randomX = Random.Range(0, _tileCount.x-1);
        int randomY = Random.Range(0, _tileCount.y-1);

        return GetGridElement(randomX, randomY);
    }

    /// <summary>
    /// Finds the edge-most cells
    /// </summary>
    /// <returns>returns a vector4 with x,X,y,Y, the minimum and maximum of each respective vector component </returns>
    public Vector4 GetTileEdges()
    {
        Vector2 minima = _gridElements[0, 0].transform.position;
        Vector2 extrema = _gridElements[_tileCount.x-1, _tileCount.y-1].transform.position;

        return new Vector4(minima.x, extrema.x, minima.y, extrema.y);
    }
}