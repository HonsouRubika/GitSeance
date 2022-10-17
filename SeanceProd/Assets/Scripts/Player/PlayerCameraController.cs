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

        [Header("Params")]
        [SerializeField] CameraTarget _startTarget;
        [HideInInspector] public CameraTarget _currentTarget;
        [SerializeField] float _cameraMovementCooldown = .3f;

        CountdownTimer _cameraMovementTimer = new CountdownTimer();
        bool _canMoveCamera = true;

        private void Start()
        {
            SwitchCameraPosition(_startTarget);
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

            if (input.x == -1f)
            {
                SetInputCooldown();
                SwitchCameraPosition(CameraTarget.Left);
                return;
            }

            if (input.x == 1f)
            {
                SetInputCooldown();
                SwitchCameraPosition(CameraTarget.Right);
                return;
			}

            int current = (int)_currentTarget;

            if(current < 2 && input.y == 1f)
            {
                current++;
                SetInputCooldown();
                SwitchCameraPositionWithIndex(current);
                return;
			}
            if(current > 0 && input.y == -1f)
            {
                current--;
                SetInputCooldown();
                SwitchCameraPositionWithIndex(current);
                return;
			}

        }

        void SwitchCameraPositionWithIndex(int index)
        {
            switch (index)
            {
                case 0:
                    SwitchCameraPosition(CameraTarget.Hand);
					break;
				case 1:
					SwitchCameraPosition(CameraTarget.Board);
					break;
				case 2:
					SwitchCameraPosition(CameraTarget.Wayfarer);
					break;
				default:
                    break;
            }
        }

        void SwitchCameraPosition(CameraTarget target)
        {
            _currentTarget = target;
            Debug.Log(_currentTarget);

            switch (_currentTarget)
            {
                case CameraTarget.Hand:
                    _animator.Play("Hand");
                    break;
                case CameraTarget.Board:
					_animator.Play("Board");
					break;
                case CameraTarget.Wayfarer:
					_animator.Play("Wayfarer");
					break;
                case CameraTarget.Left:
					_animator.Play("Left");
					break;
                case CameraTarget.Right:
					_animator.Play("Right");
					break;
                default:
                    break;
            }
        }
    }

    public enum CameraTarget
    {
        Hand = 0,
        Board = 1,
        Wayfarer = 2,
        Left = 3,
        Right = 4
    }
}
