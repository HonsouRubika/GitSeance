/// Author: Julien Haigron
/// Last modified by: Julien Haigron

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seance.TurnSystem;

namespace Seance.Interactions
{
    public class KnockKnockInteraction : Interactor
    {

        // Start is called before the first frame update
        public override void Interact(PlayerTurnState turn)
        {
            turn.PlayerKnocks();

            //play knock sound effect
            //AudioManager.Instance.PlayEffectOnTmpSource();

        }
    }
}
