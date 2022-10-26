/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Connection;
using FishNet.Object;
using Seance.Player;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Seance.TurnSystem;
using Seance.UI.Lobby;

namespace Seance.Networking
{
	public class LobbyManager : NetworkBehaviour
	{
		[Header("References")]
		[SerializeField] LobbyUIMode _lobbyUIMode;
		public LobbyUIMode LobbyUIMode { get { return _lobbyUIMode; } }

		//Server only, used temporarily to track connected clients before game start
		List<NetworkConnection> _serverConnections = new();

		//Server only, number of client set up and ready to start
		int _readyCount = 0;

		//List of all connected clients
		List<NetworkConnection> _connections = new();
		public List<NetworkConnection> Connections { get => _connections; }

		//Owned network connection of this client
		NetworkConnection _ownedConnection;
		public NetworkConnection OwnedConnection { get => _ownedConnection; set => _ownedConnection = value; }

		//Self position in _connections list
		int _ownedConnectionIndex;
		public int OwnedConnectionIndex { get => _ownedConnectionIndex; set => _ownedConnectionIndex = value; }

		//List if all player instances, setup when the game starts
		List<PlayerInstance> _playerInstances = new();
		public List<PlayerInstance> PlayerInstances { get => _playerInstances; }

		//Owned PlayerInstance object of this client
		PlayerInstance _ownedPlayerInstance;
		public PlayerInstance OwnedPlayerInstance { get => _ownedPlayerInstance; set => _ownedPlayerInstance = value; }

		//Current number of connected clients
		int _connectionCount = 0;
		public int ConnectionCount { get => _connectionCount; }

		//Called every time the connection count changes
		public static Action<int> ChangeConnectionCount;

		//Called right before the game starts, used to set all server related variables on clients
		public static Action ClientSetup;

		#region Lobby Creation

		[ServerRpc(RequireOwnership = false)]
		public void ServerAddConnection(NetworkConnection conn)
		{
			if (!IsServer)
				return;

			_serverConnections.Add(conn);

			ObserversUpdatePlayerCount(_serverConnections.Count);

			if (_serverConnections.Count == 3)
			{
				foreach (NetworkConnection connection in _serverConnections)
				{
					ObserversAddConnection(connection);
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void ServerRemoveConnection(NetworkConnection conn)
		{
			if (!IsServer)
				return;

			_serverConnections.Remove(conn);

			ObserversUpdatePlayerCount(_serverConnections.Count);

			//Pause the game if already started
		}

		public void AddPlayerInstance(PlayerInstance instance)
		{
			_playerInstances.Add(instance);

			if (_playerInstances.Count == 3)
				_playerInstances.OrderBy(index => index.LocalConnection.ClientId);
		}

		[ObserversRpc]
		public void ObserversUpdatePlayerCount(int count)
		{
			_connectionCount = count;
			ChangeConnectionCount?.Invoke(_connectionCount);
		}

		[ObserversRpc]
		public void ObserversAddConnection(NetworkConnection conn)
		{
			_connections.Add(conn);
		}

		[ObserversRpc]
		public void ObserversSetupClient()
		{
			ClientSetup?.Invoke();
		}

		[ServerRpc(RequireOwnership = false)]
		public void ServerAddPlayerReady()
		{
			if (!IsServer)
				return;

			_readyCount++;

			if (_readyCount == 3)
				ObserversStartGame();
		}

		[ObserversRpc]
		void ObserversStartGame()
		{
			GameManager.TurnStateMachine.Init();
		}

		#endregion
	}
}