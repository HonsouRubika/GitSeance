/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seance.Utility;

namespace Seance.TurnSystem
{
    public class NewChapterState : MonoState
    {
        TurnStateMachine _machine;

        Sequencer _sequencer = new();

        private void Start()
        {
            _machine = TurnStateMachine.Instance;
        }

        public override void OnStateEnter()
        {
            if (!_machine._gameStarted)
                _machine._currentChapter = _machine._gameStartChapterCards.PickRandom();
            else
                _machine._currentChapter = _machine._chapterCards.PickRandom();

            _machine.SetChapterTurnOrder(Random.Range(0, 3));
            _machine.StartPlayerTurn(0);

            _sequencer.Append(2f);
            _sequencer.Append(2f, () => _machine._currentChapter.ApplyChapterBeginEffects());
            _sequencer.Append(2f, () => _machine.StartPlayerTurn(0));

            _sequencer.Play();
        }
    }
}
