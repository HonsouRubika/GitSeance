/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Object;
using Seance.CardSystem;
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
		int _activePlayer = -1;
		public int ActivePlayer { get { return _activePlayer; } }

		bool _isPlaying;
		public bool IsPlaying { get { return _isPlaying; } }

		[Header("States")]
		[SerializeField] IntroductionState _introductionState;
		public IntroductionState IntroductionState => _introductionState;

		[SerializeField] NewChapterState _newChapterState;
		public NewChapterState NewChapterState => _newChapterState;

		[SerializeField] EndChapterState _endChapterState;
		public EndChapterState EndChapterState => _endChapterState;

		[SerializeField] PlayerTurnState _playerTurnState;
		public PlayerTurnState PlayerTurnState => _playerTurnState;

		[SerializeField] WayfarerTurnState _wayfarerTurnState;
		public WayfarerTurnState WayfarerTurnState => _wayfarerTurnState;
	}
}
