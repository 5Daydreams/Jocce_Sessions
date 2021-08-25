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
        [Header("Perlin Noise Parameters")] [SerializeField]
        private float _noiseFrequency;

        [SerializeField] private FloatValue _wallThreshold;
        [SerializeField] private IntValue _enemyCount;
        [SerializeField] private FloatValue _enemyThreshold;

        [Header("Prefabs")] [SerializeField] private GridElement _wall;
        [SerializeField] private GridElement _enemy;
        [SerializeField] private GridElement _floor;

        [ReadOnly] private Vector3 _offset = Vector2.one * 0.5f;
        private Vector2 _perlinOffset;
        private int _remainingEnemyCount;

        public override void SpawnGrid()
        {
            EraseGridElements();

            // Repositioning Grid-Generator transform to center-align the grid elements
            Vector3 recenterOffset =
                new Vector3(_cellSpacing.x * _tileCount.x, _cellSpacing.y * _tileCount.y, 0) * -0.5f;
            
            transform.position = recenterOffset;

            SetGridSize(_tileCount.x, _tileCount.y);
            _gridElements = new GridElement[_tileCount.x, _tileCount.y];
            
            _perlinOffset = new Vector2(Random.Range(10, 2000), Random.Range(10, 2000));
            _remainingEnemyCount = _enemyCount.Value;

            for (int j = 0; j < _tileCount.y; j++)
            {
                for (int i = 0; i < _tileCount.x; i++)
                {
                    // Good old Perlin noise
                    float perlinValue =
                        Mathf.PerlinNoise(_noiseFrequency * (_perlinOffset.x + i * 1.0f / _tileCount.x),
                            _noiseFrequency * (_perlinOffset.y + j * 1.0f / _tileCount.y));
                    
                    // Scaling positions to "glue" grid cells together
                    Vector3 cellPosition = new Vector3(i, j, 0.0f).ComponentMultiply(_cellSpacing);

                    GridElement elementPrefab = SetupCellToSpawn(perlinValue);

                    // There is always a floor (otherwise enemies will create holes in the grid)
                    GridElement floor = Instantiate(
                        _floor,
                        transform.position + cellPosition + _offset.ComponentMultiply(_cellSpacing),
                        _floor.transform.rotation,
                        this.gameObject.transform);
                    
                    // Scaling the transforms to avoid cell overlaps
                    floor.transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing);
                    
                    if (elementPrefab.Type != GridElementType.floor)
                    {
                        _gridElements[i, j] = Instantiate(
                            elementPrefab, 
                            transform.position + cellPosition + _offset.ComponentMultiply(_cellSpacing),
                            elementPrefab.transform.rotation,
                            this.gameObject.transform);
                        _gridElements[i, j].transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing);
                    }
                    else
                    {
                        _gridElements[i, j] = floor;
                    }
                }
            }

            // This has to be done after setting up all elements, otherwise you'll either
            // 1. get a null reference error
            // 2. or require a lot of mathematical conundrum to get the cell position values
            SetGridBoundaryValues();
            
            void SetGridBoundaryValues()
            {
                _gridBoundaries.Value = new float[4];
                _gridBoundaries.Value[0] = _gridElements[0, 0].transform.position.x;
                _gridBoundaries.Value[1] = _gridElements[_tileCount.x - 1, _tileCount.y - 1].transform.position.x;
                _gridBoundaries.Value[2] = _gridElements[0, 0].transform.position.y;
                _gridBoundaries.Value[3] = _gridElements[_tileCount.x - 1, _tileCount.y - 1].transform.position.y;    
            }
        }

        private GridElement SetupCellToSpawn(float value)
        {
            if (value >= _enemyThreshold.Value && _remainingEnemyCount > 0)
            {
                float enemyProbabilityRoll = Random.Range(0, 100.0f);
                if (enemyProbabilityRoll < 90.0f)
                    return _floor;
                
                _remainingEnemyCount--;
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

            PlayerInput.Instance.RepositionPlayer(randomCell.transform.position);
            PlayerInput.Instance.transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing) * 0.9f;
        }

        private void EraseGridElements()
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