/// Author: Haigron Julien
/// Last modified by: Haigron Julien
using UnityEngine;
using Seance.Player;

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
        public float _hoveringHandYPosOffset = 5f;
        public float _cardXRotOnHand = -30f;

        //back
        public int _cost;
        public PlayerCardZones _originZone;


        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _bc = GetComponent<BoxCollider>();
            _boardController = BoardController.instance;

            _startYPos = _boardController.transform.position.y + 0.5f;

            //reset default physics params
            if (!Physics.autoSimulation) Physics.autoSimulation = true;
        }

        public override void Use()
        {
            //

        }

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

            //back
            _originZone._currentCardSeleted = this;

            //actualise cards in hand position and rotation
            _originZone.ActualiseCardsPositionInHand(this);
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
            }
            else
            {
                //reset rot
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                //reset pos
                Vector3 newWorldPosition = new Vector3(_boardController._currentMousePosition.x, _startYPos, _boardController._currentMousePosition.z);
                transform.position = newWorldPosition;

                //card is on board, trigger Use fnct
                //Use();
            }

            //reset velocity
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);

            ///back
            _originZone._currentCardSeleted = null;
        }


    }
}
