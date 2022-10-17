/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Connection;
using FishNet.Object;
using Seance.Player;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Seance.Networking
{
	public class LobbyManager : NetworkBehaviour
	{
		//Server only, used temporarily to track connected clients before game start
		List<NetworkConnection> _serverConnections = new();

		//List of all connected clients
		[HideInInspector] public List<NetworkConnection> _connections = new();

		//Self position in _connections list
		[HideInInspector] public int _ownedConnectionReferencePosition;

		//Owned network connection of this client
		[HideInInspector] public NetworkConnection _ownedConnection;

		//List if all player instances, setup when the game starts
		[HideInInspector] public List<PlayerInstance> _playerInstances = new();

		//Owned PlayerInstance object of this client
		[HideInInspector] public PlayerInstance _ownedPlayerInstance;

		//Current number of connected clients
		[HideInInspector] public int _connectionCount = 0;

		//Called every time the connection count changes
		public static Action<int> OnConnectionCountChange;

		//Called right before the game starts, used to set all server related variables on clients
		public static Action OnClientSetup;

		//Number of client set up and ready to start
		int _readyCount = 0;

		#region Singleton

		public static LobbyManager Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion

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
			OnConnectionCountChange?.Invoke(_connectionCount);
		}

		[ObserversRpc]
		public void ObserversAddConnection(NetworkConnection conn)
		{
			_connections.Add(conn);
		}

		[ObserversRpc]
		public void ObserversSetupClient()
		{
			OnClientSetup?.Invoke();
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
			//Init state machine
		}

		#endregion
	}
}