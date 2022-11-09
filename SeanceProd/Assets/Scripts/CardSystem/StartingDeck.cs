/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using UnityEngine;

namespace Seance.CardSystem
{
    [CreateAssetMenu(fileName = "New Starting Deck", menuName = "Scriptable Objects/Card System/Starting Deck", order = 50)]
    public class StartingDeck : ScriptableObject
    {
        public List<ActionCard> _cards;
    }
}
