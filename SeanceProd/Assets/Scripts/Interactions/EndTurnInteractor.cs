/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.TurnSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Interactions
{
    public class EndTurnInteractor : Interactor
    {
        public override void Interact(PlayerTurnState turn)
        {
            Debug.LogError("End Turn");
            turn.EndTurn();
        }
    }
}
