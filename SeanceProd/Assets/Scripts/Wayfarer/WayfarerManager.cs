/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Networking;
using Seance.TurnSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Wayfarer
{
    public class WayfarerManager : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] float _headMovementTime;

        [Header("References")]
        [SerializeField] Transform _renderer;
        [SerializeField] Transform[] _positions;

        QuaternionLerper _lerper = new();

        LobbyManager _lobby;

		#region Singleton

		public static WayfarerManager Instance;

		private void Awake()
		{
			Instance = this;
		}

        #endregion

        private void Start()
        {
            _lobby = LobbyManager.Instance;
        }

        public void MoveToPosition(int positionIndex)
        {
            Quaternion origin = _renderer.rotation;

            int index = _lobby._playerInstances[positionIndex]._worldPositionIndex;

            _lerper.Lerp(origin, _positions[index].rotation, _headMovementTime, delta => _renderer.rotation = delta);
        }
    }
}