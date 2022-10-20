/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Animator _animator;
        [SerializeField] Camera _camera;
        public Camera Camera { get { return _camera; } }

        [Header("Params")]
        [SerializeField] Vector2Int _startPosition;
        [HideInInspector] public Vector2Int _currentPosition = new Vector2Int(-1, -1);
        [HideInInspector] public string[,] _positions = new string[3,3];

        [SerializeField] float _cameraMovementCooldown = .15f;

        CountdownTimer _cameraMovementTimer = new CountdownTimer();
        bool _canMoveCamera = true;

        private void Start()
        {
            _positions[0, 0] = "BottomLeft";
            _positions[1, 0] = "PlayerSpace";
            _positions[2, 0] = "BottomRight";

            _positions[0, 1] = "Left";
            _positions[1, 1] = "Wayfarer";
            _positions[2, 1] = "Right";

            _positions[1, 2] = "Boardfocus";

			SwitchCameraPosition(_startPosition);
        }

        void SetInputCooldown()
        {
			_canMoveCamera = false;
			_cameraMovementTimer.SetTime(_cameraMovementCooldown, () => _canMoveCamera = true);
		}

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!_canMoveCamera)
                return;

            Vector2 input = context.ReadValue<Vector2>();

            Vector2Int current = _currentPosition;

            if (input.x == 1f && _currentPosition.x < 2)
            {
                current.x++;

                if (current.x == 2 && current.y == 2)
                    current.y = 1;

                SwitchCameraPosition(current);
            }
            else if(input.x == -1f && _currentPosition.x > 0)
            {
				current.x--;

				if (current.x == 0 && current.y == 2)
					current.y = 1;

				SwitchCameraPosition(current);
			}
            else if(input.y == 1f && _currentPosition.y < 2)
            {
                current.y++;

                if (current.y == 2)
                    current.x = 1;

                SwitchCameraPosition(current);
            }
            else if(input.y == -1f && _currentPosition.y > 0)
            {
				current.y--;

				SwitchCameraPosition(current);
			}

        }

        void SwitchCameraPosition(Vector2Int newPosition)
        {
            if (newPosition == _currentPosition)
                return;

			SetInputCooldown();

            _currentPosition = newPosition;
            _animator.Play(_positions[_currentPosition.x, _currentPosition.y]);
		}
	}
}
