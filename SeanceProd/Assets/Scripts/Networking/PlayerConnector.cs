using FishNet.Managing;
using System.Collections;
using UnityEngine;

namespace Seance.Networking
{
    public class PlayerConnector : MonoBehaviour
    {
		[Header("References")]
		[SerializeField] LobbyManager _lobbyManager;
		[SerializeField] NetworkManager _networkManager;

		[Header("Params")]
		[SerializeField] ushort _serverPort;

		#region Singleton

		public static PlayerConnector Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion

        public void CreateLobby()
		{
			_networkManager.ServerManager.StartConnection();
			_networkManager.ClientManager.StartConnection("localhost", _serverPort);
		}

		public void JoinLobby(string ip)
		{
			_networkManager.ClientManager.StartConnection(ip, _serverPort);
		}
	}
}
