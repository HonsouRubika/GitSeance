/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.TurnSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Interactions
{
    public abstract class Interactor : MonoBehaviour
    {
        public abstract void Interact(PlayerTurnState turn);
    }
}
