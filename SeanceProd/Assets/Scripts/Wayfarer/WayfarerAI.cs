/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Object;
using Seance.Networking;
using UnityEngine;

namespace Seance.Wayfarer
{
	public class WayfarerAI : NetworkBehaviour
	{
		[Header("Params")]
		[SerializeField] float _headMovementTime;

		[Header("References")]
		[SerializeField] Transform _renderer;
		[SerializeField] Transform[] _positions;

		LobbyManager _lobby;

		QuaternionLerper _lerper = new();
		int _currentTarget = -1;
		public int CurrentTarget { get => _currentTarget; }
		bool _isRotating;
		public bool IsRotating { get => _isRotating; }

		private void Start()
		{
			_lobby = GameManager.Lobby;
		}

		public void MoveToPosition(int positionIndex)
		{
			MoveToPosition(positionIndex, false);
		}

		public void MoveToPosition(int positionIndex, bool forcePosition)
		{
			if (positionIndex == _currentTarget && !forcePosition)
				return;

			if (_isRotating)
				_lerper.Cancel();

			_isRotating = true;

			_currentTarget = positionIndex;

			Quaternion origin = _renderer.rotation;

			int index = _lobby.PlayerInstances[positionIndex].WorldPositionIndex;

			_lerper.Lerp(origin, _positions[index].rotation, _headMovementTime, delta => _renderer.rotation = delta, () => _isRotating = false);
		}
	}
}