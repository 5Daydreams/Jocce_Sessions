using _Code.Movement;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Code.Grid.ElementTypes
{
    [RequireComponent(typeof(Collider2D), typeof(MovePointController))]
    public class GridEnemy : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private MovePointController _movePointController;
        [SerializeField] private UnityEvent _collisionEvent;

        private const float BASE_ROTATION_VALUE = 90.0f;

        private void Awake()
        {
            if (_collider2D.Equals(null))
            {
                _collider2D = GetComponent<Collider2D>();
            }

            if (_movePointController.Equals(null))
            {
                _movePointController = GetComponent<MovePointController>();
            }
        }

        private void OnDestroy()
        {
            Destroy(_movePointController.MovePoint.gameObject);
        }

        public void MoveEnemyToRandomDirection(bool forceReposition = false)
        {
            float rotationAngle = Random.Range(0, 4) * BASE_ROTATION_VALUE;
            
            // DISCLAIMER: I don't know Quaternions :D But...
            // In order to use them as a tool for generating rotations, the basic workflow is:
            // 
            // 1. Find the Axis you want to rotate around (in my case below, it's the Z axis) 
            // 2. Find how much (in angles!!) you want to rotate around that axis
            // 3. toss the previously mentioned values into Quaternion.AngleAxis(angle here , axis here )
            // 
            // Do I know why it works? Not really. I am treating them like gravity, I know I can use them like so, 
            // and that's good enough for me :p
            Vector2 direction = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * Vector2.right;

            if (_movePointController.CheckForFreeSpace(direction, 0.5f))
            {
                _movePointController.RepositionMovePoint(this.transform.position, direction, forceReposition);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == this.gameObject.layer)
            {
                MoveEnemyToRandomDirection(true);
                return;
            }
            _collisionEvent.Invoke();
        }
    }
}