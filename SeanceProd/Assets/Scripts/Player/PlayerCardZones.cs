/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using UnityEngine;
using Seance.CardSystem;
using System.Linq;
using Seance.Utility;
using System;

namespace Seance.Player
{
    public class PlayerCardZones : MonoBehaviour
    {
        public List<PlayCard> _deck = new();
        public List<PlayCard> _hand = new();
        public List<PlayCard> _discard = new();

        public void Init(List<PlayCard> cards)
        {
            foreach (PlayCard card in cards)
            {
                _deck.Add(Instantiate(card));
            }
        }

        public void RebuildDeck()
        {
            _deck.Concat(_discard);
            _discard.Clear();
            _deck = _deck.FisherYates();
        }

        public void PickCards(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < count; i++)
            {
                PickCard();
            }
        }

        public void PickCard()
        {
            if (_deck.Count <= 0)
                RebuildDeck();

            _hand.Add(_deck[0]);
            _deck.RemoveAt(0);
        }

        public void UseCard(int cardIndex)
        {
            if (_hand.Count <= 0)
                throw new EmptyCollectionException();

            if (cardIndex < 0 || cardIndex >= _hand.Count)
                throw new ArgumentOutOfRangeException();

            UseCard(_hand[cardIndex]);
        }

        public void UseCard(PlayCard card)
        {
            card.ApplyCardEffects();

            DiscardFromHand(card);
        }

        public void DiscardFromHand(int cardIndex)
        {
			if (_hand.Count <= 0)
				throw new EmptyCollectionException();

			if (cardIndex < 0 || cardIndex >= _hand.Count)
				throw new ArgumentOutOfRangeException();

            DiscardFromHand(_hand[cardIndex]);
		}

        public void DiscardFromHand(PlayCard card)
        {
			_discard.Add(card);
			_hand.Remove(card);
		}

        public void DiscardFromDeck(int cardIndex)
        {
			if (_deck.Count <= 0)
				throw new EmptyCollectionException();

			if (cardIndex < 0 || cardIndex >= _deck.Count)
				throw new ArgumentOutOfRangeException();

			DiscardFromDeck(_deck[cardIndex]);
		}

        public void DiscardFromDeck(PlayCard card)
        {
			_discard.Add(card);
			_deck.Remove(card);
		}
    }
}