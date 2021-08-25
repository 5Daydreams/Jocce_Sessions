using System;
using _Code.Extensions;
using _Code.Scriptables;
using TreeEditor;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Code.Grid
{
    public class PerlinNoiseGrid : SpawnInRectangularGrid<GridElement>
    {
        [Header("Perlin Noise Parameters")]
        [SerializeField] private float _noiseFrequency;
        [SerializeField] private FloatValue _wallThreshold;
        [SerializeField] private FloatValue _enemyThreshold;

        [Header("Prefabs")]
        [SerializeField] private GridElement _wall;
        [SerializeField] private GridElement _enemy;
        [SerializeField] private GridElement _floor;

        [ReadOnly] private Vector3 _offset = Vector2.one * 0.5f;
        private Vector2 _randomOffset;

        public override void SpawnGrid()
        {
            if (!_gridElements.Equals(null))
                DeleteCurrentGrid();
            
            //Repositioning transform to center-align grid elements
            Vector3 recenterOffset = new Vector3(_cellSpacing.x * _tileCount.x, _cellSpacing.y * _tileCount.y, 0) * -0.5f;
            transform.position = recenterOffset;
            
            SetGridSize(_tileCount.x,_tileCount.y);
            _gridElements = new GridElement[_tileCount.x,_tileCount.y];
            
            _randomOffset = new Vector2(Random.Range(10, 2000),Random.Range(10, 2000));

            for (int j = 0; j < _tileCount.y; j++)
            {
                for (int i = 0; i < _tileCount.x; i++)
                {
                    float perlinValue =
                        Mathf.PerlinNoise(_noiseFrequency * (_randomOffset.x + i * 1.0f / _tileCount.x),
                            _noiseFrequency * (_randomOffset.y + j * 1.0f / _tileCount.y));
                    Vector3 cellPosition = new Vector3(i, j, 0.0f).ComponentMultiply(_cellSpacing);

                    GridElement elementPrefab = SetupCellToSpawn(perlinValue);

                    _gridElements[i, j] = Instantiate(
                        elementPrefab,
                        transform.position + cellPosition + _offset.ComponentMultiply(_cellSpacing),
                        Quaternion.identity,
                        this.gameObject.transform);
                    _gridElements[i, j].transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing);
                }
            }
            
            _gridBoundaries.Value = new float[4];
            _gridBoundaries.Value[0] = _gridElements[0, 0].transform.position.x;
            _gridBoundaries.Value[1] = _gridElements[_tileCount.x - 1, _tileCount.y - 1].transform.position.x;
            _gridBoundaries.Value[2] = _gridElements[0, 0].transform.position.y;
            _gridBoundaries.Value[3] = _gridElements[_tileCount.x - 1, _tileCount.y - 1].transform.position.y;
        }

        private GridElement SetupCellToSpawn(float value)
        {
            if (value >= _enemyThreshold.Value)
            {
                return _enemy;
            }

            if (value < _wallThreshold.Value)
            {
                return _wall;
            }

            return _floor;
        }

        public void SpawnPlayer()
        {
            var randomCell = GetRandomGridElement();

            while (randomCell.Type != GridElementType.floor)
            {
                randomCell = GetRandomGridElement();
            }

            Player.Instance.RepositionPlayer(randomCell.transform.position);
            Player.Instance.transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                SpawnGrid();
                SpawnPlayer();
            }
        }
    }
}