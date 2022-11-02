using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.CardSystem
{
    public class BoardController : MonoBehaviour
    {
        [HideInInspector] public Vector3 _currentMousePosition;
        private Camera _mainCamera;
        public bool _isHoveringHand;

        //Singleton
        public static BoardController instance;

        #region Singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);

            instance = this;
        }
        #endregion

        private void Start()
        {
            _mainCamera = Camera.main; // TODO : Get curent camera
            _isHoveringHand = false;
        }

        private void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Table")) continue;
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);

                if (hit.collider.gameObject.tag == "Hand")
                    _isHoveringHand = true;
                else
                    _isHoveringHand = false;

                _currentMousePosition = hit.point;

                break;
            }
        }
    }
}
