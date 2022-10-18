/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.CardSystem;
using Seance.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.TurnSystem
{
    public class TurnStateMachine : MonoStateMachine
    {
        [Header("Params")]
		public StartingDeck[] _startingDecks;

		#region Singleton

		public static TurnStateMachine Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion
	}
}
