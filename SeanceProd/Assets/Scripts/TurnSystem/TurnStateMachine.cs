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
		[Space]
		public List<ChapterCard> _gameStartChapterCards = new();
		public List<ChapterCard> _chapterCards = new();

		[HideInInspector] public ChapterCard _currentChapter;

		[HideInInspector] public bool _gameStarted = false;
		int _activePlayer;
		public int ActivePlayer { get { return _activePlayer; } }

		[HideInInspector] public int[] _chapterTurnOrder = new int[4];

		public void SetChapterTurnOrder(int firstPlayerIndex)
		{
			_chapterTurnOrder[0] = firstPlayerIndex;
			for (int i = 1; i < 3; i++)
			{
				firstPlayerIndex++;
				if (firstPlayerIndex > 2)
					firstPlayerIndex = 0;

				_chapterTurnOrder[i] = firstPlayerIndex;
			}

			_chapterTurnOrder[3] = 3;
			_activePlayer = 0;
		}

		public void StartPlayerTurn(int playerIndex)
		{
			if (_chapterTurnOrder[_activePlayer] != 3)
			{
				//send new turn command to player _chapterTurnOrder[_activePlayer]
			}
			else
			{
				//start wayfarer turn
			}
		}

		public void StartNextPlayerTurn()
		{
			_activePlayer++;
			if (_activePlayer > 3)
			{
				_activePlayer = 0;
			}

			StartPlayerTurn(_activePlayer);
		}


		#region Singleton

		public static TurnStateMachine Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion
	}
}
