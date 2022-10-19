/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Object;
using Seance.Level;
using Seance.Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using Seance.CardSystem;
using Seance.TurnSystem;
using System.Collections.Generic;

namespace Seance.Player
{
	public class PlayerInstance : NetworkBehaviour
	{
		LobbyManager _lobby;
		LevelReferences _levelReferences;

		[Header("References")]
		[SerializeField] GameObject _cameraParent;
		[SerializeField] PlayerInputManager _inputManager;
		public PlayerInputManager InputManager { get { return _inputManager; } }
		public GameObject CameraParent { get { return _cameraParent; } }
		[SerializeField] PlayerCameraController _cameraController;
		public PlayerCameraController CameraController { get { return _cameraController; } }
		[SerializeField] PlayerInput _input;
		[Space]
		[SerializeField] PlayerCardZones _zones; 

		#region Connection to server

		public override void OnStartClient()
		{
			base.OnStartClient();

			_lobby = LobbyManager.Instance;
			_levelReferences = LevelReferences.Instance;

			_lobby.AddPlayerInstance(this);

			if (!IsOwner)
				return;

			LobbyManager.OnClientSetup += SetupClient;
			_lobby._ownedConnection = LocalConnection;
			_lobby._ownedPlayerInstance = this;
			_lobby.ServerAddConnection(LocalConnection);
		}

		public override void OnStopClient()
		{
			base.OnStopClient();

			if (!IsOwner)
				return;

			_lobby.ServerRemoveConnection(LocalConnection);
		}

		void SetupClient()
		{
			if (!IsOwner)
			{
				Destroy(_cameraParent.gameObject);
				Destroy(_input);
				return;
			}

			//Find and set OwnedConnectionReferencePosition

			for (int i = 0; i < _lobby._connections.Count; i++)
			{
				if (_lobby._connections[i].ClientId == _lobby._ownedConnection.ClientId)
				{
					_lobby._ownedConnectionReferencePosition = i;
					break;
				}
			}

			//Set position of players

			int positionIndex = _lobby._ownedConnectionReferencePosition;

			transform.position = _levelReferences._activePlayerTransform.position;
			transform.rotation = _levelReferences._activePlayerTransform.rotation;

			positionIndex++;
			if (positionIndex > 2)
				positionIndex = 0;

			_lobby._playerInstances[positionIndex].transform.position = _levelReferences._leftPlayerTransform.position;
			_lobby._playerInstances[positionIndex].transform.rotation = _levelReferences._leftPlayerTransform.rotation;

			positionIndex++;
			if (positionIndex > 2)
				positionIndex = 0;

			_lobby._playerInstances[positionIndex].transform.position = _levelReferences._rightPlayerTransform.position;
			_lobby._playerInstances[positionIndex].transform.rotation = _levelReferences._rightPlayerTransform.rotation;

			//Set starting deck for this player

			_zones.Init(TurnStateMachine.Instance._startingDecks[_lobby._ownedConnectionReferencePosition]._cards);

			//Enable camera and set state to 'ready'

			_cameraParent.SetActive(true);

			_lobby.ServerAddPlayerReady();
		}

		#endregion
	}
}
