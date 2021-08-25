using _Code.Extensions;
using _Code.Scriptables;
using UnityEngine;

namespace _Code.Movement
{
    public class MovePointController : MonoBehaviour
    {
        [SerializeField] private FloatArrayValue _gridBoundaries;
        [SerializeField] private FloatArrayValue _gridScale;
        [SerializeField] private float _speedFactor;
        private float _distanceToMovePoint;

        [HideInInspector] public Transform MovePoint;
        
        private const float MOVEPOINT_DISTANCE_THRESHOLD = 0.0005f;
        
        public bool WithinReach => _distanceToMovePoint < MOVEPOINT_DISTANCE_THRESHOLD;
        
        private void Awake()
        {
            MovePoint = new GameObject("Move Point").transform;
            MovePoint.transform.position = this.transform.position;
        }

        public bool CheckIfMovementPossible(Vector2 displacementDirection)
        {
            float averageDistance = (_gridScale.Value[0] + _gridScale.Value[1])/2.5f;
            
            // Check for colliders: if you find any, that's an invalid direction, which returns false
            return !Physics2D.Raycast(transform.position, displacementDirection,averageDistance);
        }

        public void TryRepositionMovePoint(Vector3 position, Vector2 displacementDirection)
        {
            if ( _distanceToMovePoint >= MOVEPOINT_DISTANCE_THRESHOLD)
            {
                return;
            }
            
            Vector3 gridScale = new Vector3(_gridScale.Value[0], _gridScale.Value[1], 0);
            Vector3 testMovement = position + ((Vector3)displacementDirection).ComponentMultiply(gridScale);

            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(testMovement.x,_gridBoundaries.Value[0],_gridBoundaries.Value[1]), 
                Mathf.Clamp(testMovement.y,_gridBoundaries.Value[2],_gridBoundaries.Value[3]) , 
                0.0f);
            
            MovePoint.position = clampedPosition;
        }
        
        public void LerpTowardsMovePoint()
        {
            var movePointPosition = MovePoint.position;
            this.transform.position = Vector3.Lerp(
                this.transform.position,
                movePointPosition,
                _speedFactor * Time.deltaTime);
            
            _distanceToMovePoint = Vector3.Distance(movePointPosition, transform.position);
        }
    }
}