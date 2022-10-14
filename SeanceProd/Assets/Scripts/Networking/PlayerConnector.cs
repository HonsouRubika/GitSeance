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

        private void Update()
        {
			if(Input.GetKeyDown(KeyCode.C))
            {
				CreateLobby();
            }

            if(Input.GetKeyDown(KeyCode.J))
            {
				JoinLobby("localhost");
            }
        }

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
