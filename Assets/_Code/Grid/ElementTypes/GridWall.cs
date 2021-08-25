using System;
using UnityEngine;

namespace _Code.Grid
{
    [RequireComponent(typeof(Collider2D))]
    public class GridWall : GridElement
    {
        [SerializeField] private Collider2D _collider2D;
        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
        }
    }
}