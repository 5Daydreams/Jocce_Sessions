using _Code._Scriptables.ValueTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Code.Grid.BaseTypes
{
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
        
            _gridElements = new T[_tileCount.x, _tileCount.y];
        
            _cellSpacingValues.Value = new float[2];
            _cellSpacingValues.Value[0] = _cellSpacing.x;
            _cellSpacingValues.Value[1] = _cellSpacing.y;
        }

        public virtual T GetGridElement(int x, int y)
        {
            return _gridElements[x, y];
        }

        public virtual T GetRandomGridElement()
        {
            int randomX = Random.Range(0, _tileCount.x-1);
            int randomY = Random.Range(0, _tileCount.y-1);

            return GetGridElement(randomX, randomY);
        }
    
        protected void EraseGridElements()
        {
            if (transform.childCount > 0)
                DestroyAllChildren();

            void DestroyAllChildren()
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = transform.GetChild(i);
                    Destroy(child.gameObject);
                }
            }
        }
    }
}