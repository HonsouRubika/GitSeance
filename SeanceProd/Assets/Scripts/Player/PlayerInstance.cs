/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Object;
using Seance.Level;
using Seance.Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.Player
{
	public class PlayerInstance : NetworkBehaviour
	{
		[Header("References")]
		[SerializeField] PlayerInputHandler _inputHandler;
		public PlayerInputHandler InputHandler { get { return _inputHandler; } }

		[SerializeField] PlayerCameraController _cameraController;
		public PlayerCameraController CameraController { get { return _cameraController; } }

		[SerializeField] Transform _cameraParent;
		public Transform CameraParent { get => _cameraParent; }

		[SerializeField] PlayerCardZones _zones;
		public PlayerCardZones Zones { get { return _zones; } }

		[HideInInspector] private int worldPositionIndex;
		public int WorldPositionIndex { get => worldPositionIndex; set => worldPositionIndex = value; }

		[Space]
		LobbyManager _lobby;
		LevelElements _levelElements;
		[SerializeField] PlayerInput _playerInput;

		#region Connection to server

		public override void OnStartClient()
		{
			base.OnStartClient();

			_lobby = GameManager.Lobby;
			_levelElements = GameManager.LevelElements;

			_lobby.AddPlayerInstance(this);

			if (!IsOwner)
				return;

			LobbyManager.ClientSetup += SetupClient;
			_lobby.OwnedConnection = LocalConnection;
			_lobby.OwnedPlayerInstance = this;
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
				Destroy(_playerInput);
				return;
			}

			_playerInput.enabled = true;

			//Find and set OwnedConnectionReferencePosition

			for (int i = 0; i < _lobby.Connections.Count; i++)
			{
				if (_lobby.Connections[i].ClientId == _lobby.OwnedConnection.ClientId)
				{
					_lobby.OwnedConnectionIndex = i;
					break;
				}
			}

			//Set position of players

			int positionIndex = _lobby.OwnedConnectionIndex;

			for (int i = 0; i < 3; i++)
			{
				_lobby.PlayerInstances[positionIndex].transform.position = _levelElements.PlayersSpawnPositions[i].position;
				_lobby.PlayerInstances[positionIndex].transform.rotation = _levelElements.PlayersSpawnPositions[i].rotation;
				_lobby.PlayerInstances[positionIndex].WorldPositionIndex = i;

				positionIndex++;
				if (positionIndex > 2)
					positionIndex = 0;
			}

			//Set starting deck for this player

			_zones.Init(GameManager.TurnStateMachine._startingDecks[_lobby.OwnedConnectionIndex]._cards);
			///TODO : Link to new system
			//_zones.Init(TurnStateMachine.Instance._startingDecks[_lobby._ownedConnectionReferencePosition]._cards);

			//Enable camera and set state to 'ready'

			_cameraParent.gameObject.SetActive(true);

			_lobby.ServerAddPlayerReady();
		}

		#endregion
	}
}
