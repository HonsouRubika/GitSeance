/// Author: Nicolas Capelier
/// Last modified by: Julien Haigron

using Seance.Interactions;
using Seance.Networking;
using Seance.Utility;
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

        [Header("Params")]
        [SerializeField] LayerMask _interactableLayer;

        private void Start()
        {
            _machine = TurnStateMachine.Instance;
            _lobby = LobbyManager.Instance;
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
            //player has cheated trigger
            Debug.Log("player changed dice value");
        }

        public void PlayerKnocks()
        {
            //player get wayfarer focus
            Debug.Log("player knocks");
        }
    }
}
