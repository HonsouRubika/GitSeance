/// Author: Nicolas Capelier
/// Last modified by: Haigron Julien

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
        [Header("Back")]
        public List<ActionCard> _deck = new();
        public List<ActionCard> _hand = new();
        public List<ActionCard> _discard = new();

        public List<CardScriptableObject> _cardsInformations;
        public GameObject _cardPrefab;
        public TmpPlayer _player;

        public bool _isPlayerDraggingCard;

        [Space]
        [Header("Front")]
        public Transform _deckPosition;
        public Transform _handPosition;
        public float _defaultOffsetYCardsOnDeck = 1f;
        public float _defaultOffsetXCardsOnHand = 1f;
        public float _defaultOffsetYCardsOnHand = 0.1f;
        public float _defaultOffsetXHoveringCardOnHand = 0.5f;
        public float _defaultOffsetYHoveringCardOnHand = 1f;
        public float _defaultRotationOffsetCardsOnHand = 6f;

        /* DEPRECIATED
        public void Init(List<ActionCard> cards)
        {
            foreach (ActionCard card in cards)
            {
                _deck.Add(Instantiate(card));
            }
        }*/

        private void Start()
        {
            //instantiate deck
            Init(_cardsInformations);

            //draw hand
            PickCards(5);

            //init var
            _isPlayerDraggingCard = false;
        }

        public void Init(List<CardScriptableObject> scriptableObjectsCards)
        {
            //instantiate cards prefab in deck pile
            Vector3 thisCardPosition = _deckPosition.position;
            float currentCardYOffset = 0;

            foreach (CardScriptableObject cardInfo in scriptableObjectsCards)
            {
                GameObject card = Instantiate(_cardPrefab, new Vector3(thisCardPosition.x, thisCardPosition.y + (currentCardYOffset++ * _defaultOffsetYCardsOnDeck), thisCardPosition.z), Quaternion.identity);
                ActionCard actionCardScript = card.GetComponent<ActionCard>();
                actionCardScript._cardInfo = cardInfo;
                actionCardScript._originZone = this;

                //set card on deck physics
                card.GetComponent<BoxCollider>().isTrigger = false;

                _deck.Add(actionCardScript);
            }
        }

        public void RebuildDeck()
        {
            _deck.Concat(_discard);
            _discard.Clear();
            //_deck = _deck.FisherYates();
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
            //back
            if (_deck.Count <= 0)
                RebuildDeck();

            _hand.Add(_deck[_deck.Count - 1]);
            _deck.RemoveAt(_deck.Count - 1);

            UpdateCardsPositionInHand();
        }

        public void UpdateCardsPositionInHand()
        {
            //move new card to hand
            Vector3 startPos = new Vector3(_handPosition.position.x, _handPosition.position.y + 0.5f, _handPosition.position.z);
            float cursor = 0;
            float gridBaseXPos; //for rotation angle factor

            if (_hand.Count % 2 == 0)
            {
                startPos.x -= (_hand.Count / 2) * _defaultOffsetXCardsOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }
            else
            {
                startPos.x -= (((_hand.Count - 1) / 2) * _defaultOffsetXCardsOnHand);
                gridBaseXPos = -(_hand.Count / 2);
            }

            float angle = _cardPrefab.GetComponent<ActionCard>()._cardXRotOnHand;
            Quaternion defaultRot = Quaternion.Euler(new Vector3(angle, 0, 0));
            foreach (ActionCard card in _hand)
            {
                card.gameObject.transform.position = new Vector3(startPos.x + cursor * _defaultOffsetXCardsOnHand, startPos.y + _defaultOffsetYCardsOnHand * cursor, startPos.z);

                //default card rot
                card.gameObject.transform.rotation = defaultRot;

                ///TODO : fix set rotation
                //Quaternion cardRot = Quaternion.AngleAxis(_defaultRotationOffsetCardsOnHand * gridBaseXPos, -card.transform.forward);
                //card.gameObject.transform.rotation = cardRot;



                //set card on hand physics
                Rigidbody rb = card.GetComponent<Rigidbody>();
                rb.useGravity = false;
                card.GetComponent<BoxCollider>().isTrigger = false;

                //reset velocity
                rb.velocity = new Vector3(0, 0, 0);
                rb.angularVelocity = new Vector3(0, 0, 0);

                cursor++;
                gridBaseXPos++;
            }
        }

        public void ActualiseCardsPositionInHand(ActionCard hoveringCard)
        {
            //move new card to hand
            Vector3 startPos = new Vector3(_handPosition.position.x, _handPosition.position.y + 0.5f, _handPosition.position.z);
            float cursor = 0;
            float gridBaseXPos; //for rotation angle factor

            if (_hand.Count % 2 == 0)
            {
                startPos.x -= (_hand.Count / 2) * _defaultOffsetXCardsOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }
            else
            {
                startPos.x -= (((_hand.Count - 1) / 2) * _defaultOffsetXCardsOnHand);
                gridBaseXPos = -(_hand.Count / 2);
            }

            float angle = _cardPrefab.GetComponent<ActionCard>()._cardXRotOnHand;
            Quaternion defaultRot = Quaternion.Euler(new Vector3(angle, 0, 0));

            BoardController bc = BoardController.instance;


            //1) order cards by x position in hand ASC
            IEnumerable<ActionCard> cardsInOrder = _hand.OrderBy(card => card.transform.position.x);

            //3)
            for (int i = 0; i < cardsInOrder.Count(); i++)
            {
                //reorder hand list
                //_hand[i] = cardsInOrder.ElementAt(i);

                if (cardsInOrder.ElementAt(i) == hoveringCard)
                {
                    //do nothing
                    cursor++;
                    gridBaseXPos++;
                }
                else
                {
                    cardsInOrder.ElementAt(i).gameObject.transform.position = new Vector3(startPos.x + cursor * _defaultOffsetXCardsOnHand, startPos.y + _defaultOffsetYCardsOnHand * cursor, startPos.z);

                    //default _hand[i] rot
                    cardsInOrder.ElementAt(i).gameObject.transform.rotation = defaultRot;

                    ///TODO : fix set rotation
                    //Quaternion cardRot = Quaternion.AngleAxis(_defaultRotationOffsetCardsOnHand * gridBaseXPos, -_hand[i].transform.forward);
                    //_hand[i].gameObject.transform.rotation = cardRot;

                    //set card on hand physics
                    Rigidbody rb = cardsInOrder.ElementAt(i).GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    cardsInOrder.ElementAt(i).GetComponent<BoxCollider>().isTrigger = false;

                    //reset velocity
                    rb.velocity = new Vector3(0, 0, 0);
                    rb.angularVelocity = new Vector3(0, 0, 0);

                    cursor++;
                    gridBaseXPos++;
                }
            }

            //reorder hand list
            _hand = cardsInOrder.ToList<ActionCard>();
        }

        public void ActualiseCardsPositionInHandUnselected(ActionCard hoveringCard)
        {
            Vector3 startPos = new Vector3(_handPosition.position.x, _handPosition.position.y + 0.5f, _handPosition.position.z);
            float gridBaseXPos; //for rotation angle factor

            if (_hand.Count % 2 == 0)
            {
                startPos.x -= ((_hand.Count / 2) * _defaultOffsetXCardsOnHand) + _defaultOffsetXHoveringCardOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }
            else
            {
                startPos.x -= (((_hand.Count - 1) / 2) * _defaultOffsetXCardsOnHand) + _defaultOffsetXHoveringCardOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }


            //1) move hovering card up
            int cardPosition = 0;
            for (int i = 0; i < _hand.Count; i++)
            {
                if (_hand[i] == hoveringCard)
                {
                    _hand[i].gameObject.transform.position = new Vector3(startPos.x + i * _defaultOffsetXCardsOnHand, startPos.y + _defaultOffsetYCardsOnHand, startPos.z);
                    cardPosition = i;
                }
            }

            //2) move card on the left of the hovered card a bit on the left
            for (int i = 0; i < cardPosition; i++)
            {
                //_hand[i].gameObject.transform.position = new Vector3(_hand[i].gameObject.transform.position.x - _defaultOffsetXHoveringCardOnHand, _hand[i].gameObject.transform.position.y, _hand[i].gameObject.transform.position.z);
                _hand[i].gameObject.transform.position = new Vector3((startPos.x + i * _defaultOffsetXCardsOnHand) - _defaultOffsetXHoveringCardOnHand, startPos.y + _defaultOffsetYCardsOnHand * i, startPos.z);
                Debug.Log("left : " + i);
            }

            //3) same on the right
            for (int i = cardPosition + 1; i < _hand.Count; i++)
            {
                //_hand[i].gameObject.transform.position = new Vector3(_hand[i].gameObject.transform.position.x + _defaultOffsetXHoveringCardOnHand, _hand[i].gameObject.transform.position.y, _hand[i].gameObject.transform.position.z);
                _hand[i].gameObject.transform.position = new Vector3((startPos.x + i * _defaultOffsetXCardsOnHand) + _defaultOffsetXHoveringCardOnHand, startPos.y + _defaultOffsetYCardsOnHand * i, startPos.z);
                Debug.Log("right " + i);
            }

            /*//move new card to hand
            Vector3 startPos = new Vector3(_handPosition.position.x, _handPosition.position.y + 0.5f, _handPosition.position.z);
            float cursor = 0;
            float gridBaseXPos; //for rotation angle factor

            if (_hand.Count % 2 == 0)
            {
                startPos.x -= ((_hand.Count / 2) * _defaultOffsetXCardsOnHand) + _defaultOffsetXHoveringCardOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }
            else
            {
                startPos.x -= (((_hand.Count - 1) / 2) * _defaultOffsetXCardsOnHand) + _defaultOffsetXHoveringCardOnHand;
                gridBaseXPos = -(_hand.Count / 2);
            }

            float angle = _cardPrefab.GetComponent<ActionCard>()._cardXRotOnHand;
            Quaternion defaultRot = Quaternion.Euler(new Vector3(angle, 0, 0));

            BoardController bc = BoardController.instance;

            for (int i = 0; i < _hand.Count; i++)
            {
                //reorder hand list
                //_hand[i] = cardsInOrder.ElementAt(i);

                if (_hand[i] == hoveringCard)
                    _hand[i].gameObject.transform.position = new Vector3(startPos.x + cursor * _defaultOffsetXHoveringCardOnHand, startPos.y + _defaultOffsetYHoveringCardOnHand, startPos.z);
                else
                    _hand[i].gameObject.transform.position = new Vector3(startPos.x + cursor * _defaultOffsetXCardsOnHand, startPos.y + _defaultOffsetYCardsOnHand * cursor, startPos.z);

                //default _hand[i] rot
                _hand[i].gameObject.transform.rotation = defaultRot;

                ///TODO : fix set rotation
                //Quaternion cardRot = Quaternion.AngleAxis(_defaultRotationOffsetCardsOnHand * gridBaseXPos, -_hand[i].transform.forward);
                //_hand[i].gameObject.transform.rotation = cardRot;

                //set card on hand physics
                Rigidbody rb = _hand[i].GetComponent<Rigidbody>();
                rb.useGravity = false;
                _hand[i].GetComponent<BoxCollider>().isTrigger = false;

                //reset velocity
                rb.velocity = new Vector3(0, 0, 0);
                rb.angularVelocity = new Vector3(0, 0, 0);

                cursor++;
                gridBaseXPos++;

            }*/
        }

        public int GetCardPositionInHand(ActionCard card)
        {
            for (int i = 0; i < _hand.Count; i++)
            {
                if (_hand[i] == card)
                    return i;
            }

            return -1; //error
        }

        public void UseCard(int cardIndex)
        {
            if (_hand.Count <= 0)
                throw new EmptyCollectionException();

            if (cardIndex < 0 || cardIndex >= _hand.Count)
                throw new ArgumentOutOfRangeException();

            UseCard(_hand[cardIndex]);
        }

        public void UseCard(ActionCard card)
        {
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

        public void DiscardFromHand(ActionCard card)
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

        public void DiscardFromDeck(ActionCard card)
        {
            _discard.Add(card);
            _deck.Remove(card);
        }
    }
}