/// Author: Julien Haigron
/// Last modified by: Julien Haigron

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seance.TurnSystem;

namespace Seance.Interactions
{
    public class RollDiceInterractor : Interactor
    {
        public Dice20 _dice;

        private void Start()
        {
            _dice = GetComponentInParent<Dice20>();
        }

        // Start is called before the first frame update
        public override void Interact(PlayerTurnState turn)
        {
            turn.PlayerCheatedDice();

            _dice.IncreaseDiceValue();
        }
    }
}
