/// Author: Nicolas Capelier
/// Last modified by: Julien Haigron

using Seance.Interactions;
using Seance.Networking;
using Seance.Utility;
using Seance.Wayfarer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.TurnSystem
{
    public class PlayerTurnState : MonoState
    {
        TurnStateMachine _machine;
        LobbyManager _lobby;
        WayfarerManager _wayfarer;

        [Header("Params")]
        [SerializeField] LayerMask _interactableLayer;

        [Header("References")]
        [SerializeField] AudioClip _diceClip;
        [SerializeField] AudioClip _knocksClip;


		private void Start()
        {
            _machine = TurnStateMachine.Instance;
            _lobby = LobbyManager.Instance;
            _wayfarer = WayfarerManager.Instance;
        }

        public override void OnStateEnter()
        {
            if (!_machine.IsPlaying)
                return;
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            RaycastHit hit;

            Ray ray = _lobby._ownedPlayerInstance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, .8f);

            if(Physics.Raycast(ray, out hit, 50f, _interactableLayer))
            {
                Interactor interactor;

                if(hit.transform.TryGetComponent(out interactor))
                {
                    interactor.Interact(this);
                }
            }
        }

        public void EndTurn()
        {
            if (!_machine.IsPlaying)
                return;

            _machine.ServerPlayNextTurn();
        }

        public void PlayerCheatedDice()
        {
            if(_wayfarer.CurrentTarget == _machine.ActivePlayer)
            {
                _machine.ServerSetWayfarerTarget(_lobby._ownedConnectionReferencePosition, true);
				//AudioManager.Instance.PlayEffect(_diceClip);

			}
		}

        public void PlayerKnocks()
        {
            _machine.ServerSetWayfarerTarget(_lobby._ownedConnectionReferencePosition, false);
			//AudioManager.Instance.PlayMJVoice(_knocksClip);
		}
	}
}
