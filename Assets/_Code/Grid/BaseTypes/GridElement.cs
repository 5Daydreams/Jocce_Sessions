using UnityEngine;

namespace _Code.Grid
{
    public enum GridElementType
    {
        floor,
        wall,
        enemy
    }

    public class GridElement : MonoBehaviour
    {
        public GridElementType Type;
    }
}