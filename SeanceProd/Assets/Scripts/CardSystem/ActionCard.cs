/// Author: Haigron Julien
/// Last modified by: Haigron Julien
using UnityEngine;
using Seance.Player;
using System.Collections.Generic;

namespace Seance.CardSystem
{
    public class ActionCard : Card
    {
        //front
        private Rigidbody _rb;
        private BoxCollider _bc;
        private BoardController _boardController;
        private float _startYPos;
        public float _handYPos = 4.5f;
        public float _hoveringTableYPosOffset = 3f;
        public float _hoveringHandYPosOffset = 5.5f;
        public float _cardXRotOnHand = -30f;

        //back
        public int _cost;
        public PlayerCardZones _originZone;
        private bool _isKeyDown;
        public bool _isMouseOn;


        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _bc = GetComponent<BoxCollider>();
            _boardController = BoardController.instance;

            _startYPos = _boardController.transform.position.y + 0.5f;

            //reset default physics params
            if (!Physics.autoSimulation) Physics.autoSimulation = true;

            //back
            _isKeyDown = false;
            _isMouseOn = false;
        }

        public override void Use()
        {
            //move card from hand to discard
            _originZone.DiscardFromHand(this);

            //update hand
            _originZone.UpdateCardsPositionInHand();

            //1) get target

            /*List<PlayerCardZones> targetCardZones = new List<PlayerCardZones>();
            for (int i = 0; i < _cardInfo._effects.Count; i++)
            {
                switch (_cardInfo._targets[i])
                {
                    case CardTarget.SEFL:
                        targetCardZones.Add(_originZone);
                        break;
                    case CardTarget.TARGET_ALLY:
                        targetCardZones.Add(PickTargetAlly());
                }
            }*/

            ///for test, target is always the player
            PlayerCardZones targetCardZones = _originZone;
            TmpPlayer targetPlayer = _originZone._player;
            ///

            //2) apply effect on target
            for (int i = 0; i < _cardInfo._effects.Count; i++)
            {
                switch (_cardInfo._effects[i])
                {
                    case CardEffect.DRAW:
                        targetCardZones.PickCards(_cardInfo._effectsValues[i]);
                        break;
                    case CardEffect.CLASSIQUE_DAMAGE:
                        targetPlayer.ReceiveDamage(_cardInfo._effectsValues[i]);
                        break;
                    case CardEffect.TRUE_DAMAGE:
                        targetPlayer.ReceiveTrueDamage(_cardInfo._effectsValues[i]);
                        break;
                    case CardEffect.AMRMOR_DAMAGE:
                        targetPlayer.ReceiveArmorDamage(_cardInfo._effectsValues[i]);
                        break;
                    case CardEffect.GAIN_ARMOR:
                        targetPlayer.GainArmor(_cardInfo._effectsValues[i]);
                        break;
                    case CardEffect.HEAL:
                        targetPlayer.Heal(_cardInfo._effectsValues[i]);
                        break;
                }
            }

        }

        /*private void OnMouseEnter()
        {
            _isMouseOn = true;
        }

        private void OnMouseExit()
        {
            _isMouseOn = false;

            if (_boardController._isHoveringHand)
            {
                bool didFindAnotherCardOvered = false;

                for (int i = 0; i < _originZone._hand.Count; i++)
                {
                    if (_originZone._hand[i]._isMouseOn) didFindAnotherCardOvered = true;
                }

                if (!didFindAnotherCardOvered)
                    _originZone.UpdateCardsPositionInHand();
            }
        }

        private void OnMouseOver()
        {
            if (!_isKeyDown && _boardController._isHoveringHand)
            {
                _originZone.ActualiseCardsPositionInHandUnselected(this);
            }
        }*/

        private void OnMouseDrag()
        {
            //front
            _bc.isTrigger = true;
            _rb.useGravity = true;

            Vector3 newWorldPosition = Vector3.up;

            if (_boardController._isHoveringHand)
                newWorldPosition = new Vector3(_boardController._currentMousePosition.x, _startYPos + _hoveringHandYPosOffset, _boardController._currentMousePosition.z);
            else
                newWorldPosition = new Vector3(_boardController._currentMousePosition.x, _startYPos + _hoveringTableYPosOffset, _boardController._currentMousePosition.z);

            var difference = newWorldPosition - transform.position;

            _rb.velocity = 10 * difference;

            if (_boardController._isHoveringHand)
                _rb.rotation = Quaternion.Euler(new Vector3(_rb.velocity.z + _cardXRotOnHand, 0, -_rb.velocity.x));
            else
                _rb.rotation = Quaternion.Euler(new Vector3(_rb.velocity.z, 0, -_rb.velocity.x));


            //actualise cards in hand position and rotation
            if (_boardController._isHoveringHand)
                _originZone.ActualiseCardsPositionInHand(this);
            else
                _originZone.UpdateCardsPositionInHand();
        }

        private void OnMouseDown()
        {
            _isKeyDown = true;
            _originZone._isPlayerDraggingCard = true;
        }

        private void OnMouseUp()
        {
            ///front
            _bc.isTrigger = false;

            if (_boardController._isHoveringHand)
            {
                //deactivate gravity when on hand
                _rb.useGravity = false;

                //reset rot
                transform.rotation = Quaternion.Euler(new Vector3(_cardXRotOnHand, 0, 0));

                //reset pos
                Vector3 newWorldPosition = new Vector3(_boardController._currentMousePosition.x, _handYPos, _boardController._currentMousePosition.z);
                transform.position = newWorldPosition;

                //actualise cards in hand position and rotation
                _originZone.UpdateCardsPositionInHand();
            }
            else
            {
                //reset rot
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                //reset pos
                Vector3 newWorldPosition = new Vector3(_boardController._currentMousePosition.x, _startYPos, _boardController._currentMousePosition.z);
                transform.position = newWorldPosition;

                //card is on board, trigger Use fnct
                Use();
            }

            //reset velocity
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);

            _isKeyDown = false;
            _originZone._isPlayerDraggingCard = false;
        }

    }
}
