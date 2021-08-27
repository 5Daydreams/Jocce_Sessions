using System;
using _Code.Extensions;
using _Code.Movement;
using UnityEngine;

namespace _Code
{
    [RequireComponent(typeof(MovePointController))]
    public class PlayerInput : SingletonMono<PlayerInput>
    {
        [SerializeField] private MovePointController _movePointController;
        private Vector2 _movementDirection;

        private void Update()
        {
            bool inputReceived = CheckForInput();

            if (!inputReceived || !_movePointController.WithinReach)
                return;

            if (_movePointController.CheckForFreeSpace(_movementDirection, 0.5f))
            {
                MovePlayer();
            }
        }

        private bool CheckForInput()
        {
            _movementDirection = Vector2.zero;

            // Try Keyboard Input
            _movementDirection.x = (int) Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(_movementDirection.x) < Single.Epsilon)
            {
                _movementDirection.y = (int) Input.GetAxisRaw("Vertical");
                _movementDirection.x = 0;
            }
            
            // Try Mouse Input
            if (Input.GetMouseButton(0))
            {
                SetMovementDirectionToCursor();
            }

            // check if any input was registered in the private variable
            return _movementDirection.x != 0 || _movementDirection.y != 0;
        }

        private void SetMovementDirectionToCursor()
        {
            // Convert mouse position from pixels to a [-0.5f , +0.5f] range
            Vector3 screenSize = new Vector3(Screen.width,Screen.height,0);
            Vector3 normalizedMousePosition = Input.mousePosition.ComponentDivision(screenSize);
            Vector3 centerToMouse = normalizedMousePosition - (Vector3.one * 0.5f);

            
            // Set the direction to whichever mouse component is larger
            if (Mathf.Abs(centerToMouse.x) > Mathf.Abs(centerToMouse.y))
            {
                _movementDirection.x = centerToMouse.x.Sign();
            }
            else if (Mathf.Abs(centerToMouse.y) > Mathf.Abs(centerToMouse.x))
            {
                _movementDirection.y = centerToMouse.y.Sign();
            }
        }

        private void MovePlayer()
        {
            _movePointController.RepositionMovePoint(this.transform.position,_movementDirection);
        }

        public void RepositionPlayer(Vector3 position)
        {
            this.transform.position = position;
            _movePointController.MovePoint.transform.position = position;
        }
    }
}