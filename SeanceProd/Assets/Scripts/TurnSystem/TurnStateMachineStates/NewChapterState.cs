/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using UnityEngine;
using Seance.Sequencing;

namespace Seance.TurnSystem
{
	public class NewChapterState : MonoState
	{
		TurnStateMachine _machine;

		private void Start()
		{
			_machine = GameManager.TurnStateMachine;
		}
	}
}
