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
        public ActionCard _currentCardSeleted;

        public List<CardScriptableObject> _cardsInformations;
        public GameObject _cardPrefab;

        [Space]
        [Header("Front")]
        public Transform _deckPosition;
        public Transform _handPosition;
        public float _defaultOffsetYCardsOnDeck = 1f;
        public float _defaultOffsetXCardsOnHand = 1f;
        public float _defaultOffsetYCardsOnHand = 0.1f;
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

            ActualiseCardsPositionInHand();
        }

        public void ActualiseCardsPositionInHand()
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

        public int GetHoveringSpace(ActionCard cardHovering)
        {
            BoardController bc = BoardController.instance;

            bool isLeft = true;
            bool isRight = true;

            for (int i = 0; i < _hand.Count; i++)
            {
                if (bc._currentMousePosition.x > _hand[i].transform.position.x)
                {
                    isLeft = false;
                }
            }

            if (isLeft)
            {
                Debug.Log("IsLeft");
                return 0;
            }

            for (int i = 0; i < _hand.Count; i++)
            {
                if (bc._currentMousePosition.x < _hand[i].transform.position.x)
                {
                    isRight = false;
                }
            }

            if (isRight)
            {
                Debug.Log("IsRight");
                return _hand.Count - 1;
            }


            for (int i = 0; i < _hand.Count; i++)
            {
                if (bc._currentMousePosition.x > _hand[i].transform.position.x)
                {
                    Debug.Log($"In the middle: {i}");
                    return i + 1;
                }
            }

            Debug.Log("Aled");
            return -1;

            //for (int i = 0; i < _hand.Count; i++)
            //{
            //    if (_hand[i] != cardHovering)
            //    {
            //        if (!leftFound && _hand[i].transform.position.x > bc._currentMousePosition.x)
            //        {
            //            hoveringCardPosition = 0;
            //            placeFound = true;
            //        }
            //        else if (!placeFound && !leftFound && _hand[i].transform.position.x < bc._currentMousePosition.x)
            //        {
            //            leftFound = true;
            //        }
            //        else if (!placeFound && leftFound && _hand[i].transform.position.x > bc._currentMousePosition.x)
            //        {
            //            hoveringCardPosition = i;
            //            placeFound = true;
            //        }
            //    }


            //else if(_hand[i])












            /*else if(_hand[i] == cardHovering && leftFound && _hand.Count>i+1 && _hand[i + 1].transform.position.x > bc._currentMousePosition.x)
            {
                placeFound = true;
                hoveringCardPosition = i;
            }*/
            /*else if (_hand[i] == cardHovering && leftFound && _hand.Count == i + 1)
            {
                placeFound = true;
                hoveringCardPosition = i;
            }
            else if (_hand[i] == cardHovering && leftFound && _hand[i + 1].transform.position.x >= bc._currentMousePosition.x)
            {
                placeFound = true;
                hoveringCardPosition = i;
            }*/

            //last
            //if (i == _hand.Count - 1 && !placeFound)
            //{
            //    hoveringCardPosition = i;
            //}

        }

        /*public int GetHoveringSpace(ActionCard cardHovering)
        {
            BoardController bc = BoardController.instance;

            if (_hand.Count <= 1)
            {
                return 0;
            }
            else
            {
                if(_hand[0].transform.position.x > bc._currentMousePosition.x)
                {
                    return 0;
                }

                for (int i = 1; i < _hand.Count; i++)
                {
                    if(_hand[i-1].transform.position.x < bc._currentMousePosition.x && _hand[i+1].transform.position.x > bc._currentMousePosition.x)
                    {
                        return i;
                    }
                }
            }

            return _hand.Count-1;
        }*/

        public int GetCardPositionInHand(ActionCard card)
        {
            for (int i = 0; i < _hand.Count; i++)
            {
                if (_hand[i] == card)
                    return i;
            }

            return -1; //error
        }

        /// <summary>
        /// This function does same as the upper one but doesnt take the card "exception" into account
        /// </summary>
        /// <param name="hoveringCard"></param> The card not to take care of
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

            if (bc._isHoveringHand)
            {
                //1)
                int hoveringNewPosition = GetHoveringSpace(hoveringCard);
                //Debug.Log("card position : " + hoveringCardPosition + " == " + GetCardPositionInHand(hoveringCard));

                int hoveringOldPosition = GetCardPositionInHand(hoveringCard);

                //2)
                if (GetCardPositionInHand(hoveringCard) != hoveringNewPosition)
                {
                    _hand.Insert(hoveringNewPosition, hoveringCard);

                    if (hoveringNewPosition < hoveringOldPosition)
                    {
                        _hand.RemoveAt(hoveringOldPosition - 1);
                    }
                    else
                    {
                        _hand.RemoveAt(hoveringOldPosition);
                    }


                    //_hand.Insert(hoveringCardPosition, hoveringCard);
                }

                //3)
                for (int i = 0; i < _hand.Count; i++)
                {
                    if (_hand[i] == hoveringCard)
                    {
                        //do nothing
                        cursor++;
                        gridBaseXPos++;
                    }
                    else
                    {
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
                    }
                }
            }
            else
            {
                /*foreach (ActionCard card in _hand)
                {
                    if (card != exception)
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
                    else
                    {
                        //pass to next one
                    }
                }*/
            }
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
            card.Use();

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