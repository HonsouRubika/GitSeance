/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.TurnSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        TurnStateMachine _machine;

        private void Start()
        {
            _machine = TurnStateMachine.Instance;
		}

        public void OnClick(InputAction.CallbackContext context)
        {
            _machine.PlayerTurn.OnClick(context);
        }
    }
}
