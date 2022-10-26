/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Interactions;
using Seance.Networking;
using Seance.Wayfarer;
using UnityEngine;

namespace Seance.TurnSystem
{
	public class PlayerTurnState : MonoState
	{
		TurnStateMachine _machine;

		private void Start()
		{
			_machine = GameManager.TurnStateMachine;
		}

		public override void OnStateEnter()
		{
			if (!_machine.IsPlaying)
				return;
		}

		public void EndTurn()
		{
			if (!_machine.IsPlaying)
				return;
		}
	}
}