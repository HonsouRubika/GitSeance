using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.CardSystem
{
    public abstract class Card : MonoBehaviour
    {
        public CardScriptableObject _cardInfo;

        public abstract void Use();



    }
}
