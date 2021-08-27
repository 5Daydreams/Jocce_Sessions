using System;
using System.Collections.Generic;
using _Code._Scriptables.ValueTypes;
using _Code.Extensions;
using _Code.Grid.BaseTypes;
using _Code.Grid.ElementTypes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace _Code.Grid
{
    public class Room : IComparable<Room>
    {
        public List<Vector2Int> Tiles;
        public List<Vector2Int> EdgeTiles;
        public List<Room> ConnectedRooms;
        public int RoomSize;
        public bool IsAccessibleFromMainRoom;
        public bool IsMainRoom;

        public Room() { }

        public Room(List<Vector2Int> roomTiles, GridElement[,] gridMap)
        {
            Tiles = roomTiles;
            RoomSize = Tiles.Count;
            ConnectedRooms = new List<Room>();

            EdgeTiles = new List<Vector2Int>();
            foreach (var tile in Tiles)
            {
                for (int x = tile.x - 1; x <= tile.x + 1; x++)
                {
                    for (int y = tile.y - 1; y <= tile.y + 1; y++)
                    {
                        bool isDirectlyConnected = x == tile.x || y == tile.y;
                        bool isInMapRange = (x >= 0 && x < gridMap.GetLength(0)) &&
                                            (y >= 0 && y < gridMap.GetLength(1));

                        if (!isDirectlyConnected || !isInMapRange)
                            continue;

                        if (gridMap[x, y].isWall)
                        {
                            EdgeTiles.Add(tile);
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!IsAccessibleFromMainRoom)
            {
                IsAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in ConnectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.IsAccessibleFromMainRoom)
            {
                roomB.IsAccessibleFromMainRoom = true;
            }
            else if (roomB.IsAccessibleFromMainRoom)
            {
                roomA.IsAccessibleFromMainRoom = true;
            }

            roomA.ConnectedRooms.Add(roomB);
            roomB.ConnectedRooms.Add(roomA);
        }

        public bool IsConnected(Room other)
        {
            return ConnectedRooms.Contains(other);
        }

        public int CompareTo(Room other)
        {
            return other.RoomSize.CompareTo(RoomSize);
        }
    }

    public class PerlinNoiseGrid : SpawnInRectangularGrid<GridElement>
    {
        [Header("Perlin Noise Parameters")] 
        [SerializeField] private float _noiseFrequency;
        [SerializeField] private FloatValue _wallThreshold;
        [SerializeField] private IntValue _maximumEnemyCount;
        [SerializeField] private FloatValue _enemySpawnChance;

        [Header("Prefabs")] 
        [SerializeField] private GridElement _wall;
        [SerializeField] private GridElement _floor;
        [SerializeField] private GridEnemy _enemy;
        [SerializeField] private GridExit _exit;

        [Header("Filtering Algorithm")] 
        [SerializeField] private int _wallRegionThreshold;
        [SerializeField] private int _floorRegionThreshold;
        [SerializeField] private int _corridorWidth;

        private readonly Vector3 _cellCenterOffset = Vector2.one * 0.5f;
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
            _perlinOffset = new Vector2(Random.Range(10, 2000), Random.Range(10, 2000));

            _remainingEnemyCount = _maximumEnemyCount.Value;

            for (int j = 0; j < _tileCount.y; j++)
            {
                for (int i = 0; i < _tileCount.x; i++)
                {
                    // Good old Perlin noise
                    float perlinValue = Mathf.PerlinNoise(
                        _noiseFrequency * (_perlinOffset.x + i * 1.0f / _tileCount.x),
                        _noiseFrequency * (_perlinOffset.y + j * 1.0f / _tileCount.y));

                    // Scaling positions to "glue" grid cells together
                    Vector3 cellPosition = new Vector3(i, j, 0.0f).ComponentMultiply(_cellSpacing);

                    GridElement elementPrefab = SetupCellByPositionAndNoise(cellPosition, perlinValue);

                    _gridElements[i, j] = elementPrefab;
                    _gridElements[i, j].gameObject.name = i + "," + j;

                    TrySpawnEnemy(i, j);
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

            FilterRegions();
        }

        private GridElement SetupCellByPositionAndNoise(Vector3 cellPosition, float perlinValue)
        {
            GridElement chosenTile;

            bool isWall = perlinValue < _wallThreshold.Value;
            if (isWall)
                chosenTile = _wall;
            else
                chosenTile = _floor;

            // the instantiate methods get DISGUSTINGLY hard to read without a shorter name for this
            Vector3 spawnPosition =
                transform.position + cellPosition + _cellCenterOffset.ComponentMultiply(_cellSpacing);

            GridElement spawnedPrefab = Instantiate(chosenTile, spawnPosition,
                chosenTile.transform.rotation, this.gameObject.transform);
            spawnedPrefab.transform.localScale = _cellSpacing;

            return spawnedPrefab;
        }

        private void TrySpawnEnemy(int i, int j)
        {
            var tileObject = _gridElements[i, j];

            if (tileObject.isWall || _remainingEnemyCount <= 0)
                return;

            // just a hard-coded a 0~100 die roll to spawn an enemy
            float enemyProbabilityRoll = Random.Range(0, 1.0f);

            if (enemyProbabilityRoll > _enemySpawnChance.Value)
                return;

            _remainingEnemyCount--;

            var enemy = Instantiate(_enemy, tileObject.transform.position, _enemy.transform.rotation, this.transform);
            enemy.transform.localScale = _cellSpacing;
        }

        [ContextMenu("ForceSpawnNewEnemy")]
        public void ForceSpawnEnemy()
        {
            int randomX = Random.Range(0, _tileCount.x-1);
            int randomY = Random.Range(0, _tileCount.y-1);

            if (_gridElements[randomX, randomY].isWall)
            {
                ForceSpawnEnemy();
                return;
            }
            
            var enemy = Instantiate(_enemy, _gridElements[randomX, randomY].transform.position, _enemy.transform.rotation, this.transform);
            enemy.transform.localScale = _cellSpacing;
        }

        private void FilterRegions()
        {
            List<List<Vector2Int>> wallRegions = GetRegions(true);

            foreach (List<Vector2Int> region in wallRegions)
            {
                // Regions with very few tiles...
                if (region.Count <= _wallRegionThreshold)
                {
                    for (int i = region.Count - 1; i >= 0; i--)
                    {
                        // ... get Discarded
                        ChangeTileTo(region[i], _floor);
                    }
                }
            }

            List<List<Vector2Int>> floorRegions = GetRegions(false);

            List<Room> validRooms = new List<Room>();

            foreach (List<Vector2Int> region in floorRegions)
            {
                if (region.Count <= _floorRegionThreshold)
                {
                    for (int i = region.Count - 1; i >= 0; i--)
                    {
                        ChangeTileTo(region[i], _wall);
                    }
                }
                else
                {
                    // Floor regions that aren't discarded are marked as rooms
                    validRooms.Add(new Room(region, _gridElements));
                }
            }

            validRooms.Sort();

            // Set the largest room as the main room
            // (this is completely arbitrary, and you could create any rule for deciding which room is the main room
            // but let's face it, using the 0-th index is just too practical)
            validRooms[0].IsMainRoom = true;
            validRooms[0].IsAccessibleFromMainRoom = true;

            ConnectClosestRooms(validRooms);
        }


        /// <summary>
        /// Gets a Vector2Int and a prefab to replace on the objectGrid
        /// </summary>
        /// <param name="tile">the (x,y) position in the grid to be replaced</param>
        /// <param name="replaceTo"> the prefab which will substitute the one located at (x,y)</param>
        private void ChangeTileTo(Vector2Int tile, GridElement replaceTo)
        {
            Transform cellTransform = _gridElements[tile.x, tile.y].transform;
            Destroy(_gridElements[tile.x, tile.y].gameObject);
            _gridElements[tile.x, tile.y] =
                Instantiate(replaceTo, cellTransform.position, cellTransform.rotation, this.transform);
            _gridElements[tile.x, tile.y].gameObject.name = tile.x + "," + tile.y;
            _gridElements[tile.x, tile.y].transform.localScale = _cellSpacing;
        }

        private void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
        {
            List<Room> roomListA = new List<Room>();
            List<Room> roomListB = new List<Room>();

            // Divides rooms into list A and B, where list A is not yet connected to the main room
            if (forceAccessibilityFromMainRoom)
            {
                foreach (Room room in allRooms)
                {
                    if (room.IsAccessibleFromMainRoom)
                    {
                        roomListB.Add(room);
                    }
                    else
                    {
                        roomListA.Add(room);
                    }
                }
            }
            else // if not forcing connections, just loop through all rooms for setting at least one connection
            {
                roomListA = allRooms;
                roomListB = allRooms;
            }

            int closestDistance = 0;
            Vector2Int bestTileA = new Vector2Int();
            Vector2Int bestTileB = new Vector2Int();
            Room bestRoomA = new Room();
            Room bestRoomB = new Room();
            bool possibleConnectionFound = false;

            foreach (var roomA in roomListA)
            {
                if (!forceAccessibilityFromMainRoom)
                {
                    possibleConnectionFound = false;
                    // if already connected, go to next room in the A loop
                    if (roomA.ConnectedRooms.Count > 0)
                    {
                        continue;
                    }
                }

                foreach (var roomB in roomListB)
                {
                    // if either A is connected to B or A == B, go to the next room in the B loop
                    if (roomA == roomB || roomA.IsConnected(roomB)) 
                        continue;

                    for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
                        {
                            Vector2Int tileA = roomA.EdgeTiles[tileIndexA];
                            Vector2Int tileB = roomB.EdgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int) Mathf.Pow(tileA.x - tileB.x, 2) +
                                                       (int) Mathf.Pow(tileA.y - tileB.y, 2);

                            // find the closest distance between the room edges, to make the shortest corridor later
                            if (distanceBetweenRooms < closestDistance || !possibleConnectionFound)
                            {
                                closestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }

                if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
                    CreatePassageBetweenRooms(bestRoomA, bestRoomB, bestTileA, bestTileB);
                }
            }

            if (possibleConnectionFound && forceAccessibilityFromMainRoom)
            {
                CreatePassageBetweenRooms(bestRoomA, bestRoomB, bestTileA, bestTileB);
                ConnectClosestRooms(allRooms, true);
                // (!) Note that this causes a recursion:
                // the idea is to add even if just one more room to roomListB, and then run the algorithm again
                // this is done until no more connections are possible - ie.: all rooms can now reach the main room
            }

            if (!forceAccessibilityFromMainRoom)
            {
                ConnectClosestRooms(allRooms, true);
            }
        }

        private void CreatePassageBetweenRooms(Room roomA, Room roomB, Vector2Int tileA, Vector2Int tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.cyan, 5.0f);

            List<Vector2Int> line = GetLine(tileA, tileB);

            foreach (var pos in line)
            {
                DrawCircleOfType(pos, _corridorWidth, false);
            }
        }

        private void DrawCircleOfType(Vector2Int pos, int radius, bool isWall)
        {
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if (i * i + j * j <= radius * radius)
                    {
                        int drawX = pos.x + i;
                        int drawY = pos.y + j;

                        bool inMapRange = drawX >= 0 && drawX < _gridElements.GetLength(0) &&
                                          drawY >= 0 && drawY < _gridElements.GetLength(1);

                        if (inMapRange)
                        {
                            Vector2Int tileCoord = new Vector2Int(drawX, drawY);

                            GridElement selectedPrefab;

                            if (isWall)
                                selectedPrefab = _wall;
                            else
                                selectedPrefab = _floor;

                            ChangeTileTo(tileCoord, selectedPrefab);
                        }
                    }
                }
            }
        }

        private List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
        {
            List<Vector2Int> line = new List<Vector2Int>();

            int x = from.x;
            int y = from.y;

            int dx = to.x - from.x;
            int dy = to.y - from.y;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);


            int longest = Math.Abs(dx);
            int shortest = Math.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Math.Abs(dy);
                shortest = Math.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;

            for (int i = 0; i < longest; i++)
            {
                line.Add(new Vector2Int(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;

                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }

                    gradientAccumulation -= longest;
                }
            }

            return line;
        }


        private Vector3 CoordToWorldPoint(Vector2Int tile)
        {
            Vector3 cellPosition = new Vector3(tile.x, tile.y, 0.0f);
            return transform.position + (cellPosition + _cellCenterOffset).ComponentMultiply(_cellSpacing);
        }

        private List<List<Vector2Int>> GetRegions(bool isWall)
        {
            List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
            bool[,] visitedCells = new bool[_tileCount.x, _tileCount.y];


            for (int i = 0; i < _tileCount.x; i++)
            {
                for (int j = 0; j < _tileCount.y; j++)
                {
                    if (visitedCells[i, j] == false && _gridElements[i, j].isWall == isWall)
                    {
                        List<Vector2Int> newRegion = GetRegionTiles(i, j);
                        regions.Add(newRegion);

                        foreach (var tile in newRegion)
                        {
                            visitedCells[tile.x, tile.y] = true;
                        }
                    }
                }
            }

            return regions;
        }

        private List<Vector2Int> GetRegionTiles(int startX, int startY)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            bool[,] visitedCells = new bool[_tileCount.x, _tileCount.y];
            bool isWall = _gridElements[startX, startY].isWall;

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startY));
            visitedCells[startX, startY] = true;

            while (queue.Count > 0)
            {
                Vector2Int tile = queue.Dequeue();
                tiles.Add(tile);

                for (int x = tile.x - 1; x <= tile.x + 1; x++)
                {
                    for (int y = tile.y - 1; y <= tile.y + 1; y++)
                    {
                        bool isInMapRange = (x >= 0 && x < _tileCount.x) && (y >= 0 && y < _tileCount.y);
                        bool isDirectNeighbour = (x == tile.x || y == tile.y);

                        if (!isInMapRange || !isDirectNeighbour)
                            continue;

                        bool sameTypeAndUnvisited =
                            visitedCells[x, y] == false && _gridElements[x, y].isWall == isWall;
                        if (sameTypeAndUnvisited)
                        {
                            visitedCells[x, y] = true;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }

            return tiles;
        }

        public void RecreateGrid()
        {
            SpawnGrid();
            SpawnPlayer();
            SpawnExit();
        }

        public void SpawnPlayer()
        {
            var randomCell = GetRandomGridElement();

            while (randomCell.isWall)
            {
                randomCell = GetRandomGridElement();
            }

            PlayerInput.Instance.RepositionPlayer(randomCell.transform.position);
            PlayerInput.Instance.transform.localScale = Vector3.one.ComponentMultiply(_cellSpacing) * 0.9f;
        }

        private void SpawnExit()
        {
            var randomCell = GetRandomGridElement();

            while (randomCell.isWall)
            {
                randomCell = GetRandomGridElement();
            }

            GridExit exit = Instantiate(_exit,randomCell.transform.position,_exit.transform.rotation,this.transform);
            exit.gameObject.transform.localScale = _cellSpacing * 0.9f;
        }
    }
}