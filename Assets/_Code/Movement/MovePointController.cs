using System;
using _Code._Scriptables.ValueTypes;
using _Code.Extensions;
using UnityEngine;

namespace _Code.Movement
{
    public class MovePointController : MonoBehaviour
    {
        [Header("Grid Constraints")] [SerializeField]
        private FloatArrayValue _gridBoundaries;

        [SerializeField] private FloatArrayValue _gridScale;

        [Header("Lerp Speed")] [SerializeField]
        private float _speedFactor;

        [Header("Circlecast Parameters")] [SerializeField]
        private float _circleRadius = 1.0f;

        [SerializeField] private float _circleDragDistance = 0.5f;
        [SerializeField] private LayerMask _layer;
        private float _distanceToMovePoint;

        [HideInInspector] public Transform MovePoint;

        private const float MOVEPOINT_DISTANCE_THRESHOLD = 0.0005f;

        public bool WithinReach => _distanceToMovePoint < MOVEPOINT_DISTANCE_THRESHOLD;

        private void Awake()
        {
            MovePoint = new GameObject("Move Point").transform;
            MovePoint.transform.position = this.transform.position;
        }

        private void Update() => LerpTowardsMovePoint();

        private void LerpTowardsMovePoint()
        {
            Vector3 movePointPosition = MovePoint.position;
            this.transform.position = Vector3.Lerp(
                this.transform.position,
                movePointPosition,
                _speedFactor * Time.deltaTime);

            _distanceToMovePoint = Vector3.Distance(movePointPosition, transform.position);
        }

        public bool CheckForFreeSpace(Vector2 displacementDirection, float circleRadius)
        {
            float averageGridSize = (_gridScale.Value[0] + _gridScale.Value[1]) / 2.5f;

            // Check for colliders: if you find any, that's an invalid direction, which returns false
            return !Physics2D.CircleCast(
                (Vector2) transform.position + displacementDirection.ComponentMultiply((Vector2) transform.localScale),
                _circleRadius * averageGridSize,
                displacementDirection,
                _circleDragDistance * averageGridSize,
                _layer);
        }

        public void RepositionMovePoint(Vector3 position, Vector2 displacementDirection, bool forceReposition = false)
        {
            if (!WithinReach && !forceReposition)
            {
                return;
            }

            // Constraining player movement to the inside of the grid
            Vector3 gridScale = new Vector3(_gridScale.Value[0], _gridScale.Value[1], Single.Epsilon);
            Vector3Int positionIndex = (transform.position - gridScale * 0.5f).ComponentDivision(gridScale).ConvertToVector3Int();
            Vector3 testMovement =  (positionIndex + Vector3.one*0.5f +(Vector3) displacementDirection).ComponentMultiply(gridScale);
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(testMovement.x, _gridBoundaries.Value[0], _gridBoundaries.Value[1]),
                Mathf.Clamp(testMovement.y, _gridBoundaries.Value[2], _gridBoundaries.Value[3]),
                0.0f);

            MovePoint.position = clampedPosition;
        }
    }
}