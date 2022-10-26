/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Networking;
using TMPro;
using UnityEngine;

namespace Seance.UI.Lobby
{
	public class LobbyUIMode : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] GameObject _defaultCamera;
		[SerializeField] GameObject _startGameButton;
		[SerializeField] GameObject _createLobbyButton;
		[SerializeField] GameObject _joinLobbyButton;
		[SerializeField] TextMeshProUGUI _playerCountText;
		[SerializeField] TMP_InputField _inputField;

		PlayerConnector _connector;
		LobbyManager _lobby;

		bool _isHost = false;

		private void Start()
		{
			_connector = GameManager.Connector;
			_lobby = GameManager.Lobby;

			LobbyManager.ChangeConnectionCount += OnConnectionCountChange;
			LobbyManager.ClientSetup += SetupClient;
		}

		public void CreateLobbyButton()
		{
			_isHost = true;
			_connector.CreateLobby();

			_createLobbyButton.SetActive(false);
			_joinLobbyButton.SetActive(false);
			_inputField.gameObject.SetActive(false);
		}

		public void JoinLobbyButton()
		{
			if (_inputField.text == null || _inputField.text.Length == 0)
				_connector.JoinLobby("localhost");
			else
				_connector.JoinLobby(_inputField.text);


			_createLobbyButton.SetActive(false);
			_joinLobbyButton.SetActive(false);
			_inputField.gameObject.SetActive(false);
		}

		public void StartGameButton()
		{
			_lobby.ObserversSetupClient();
		}

		void SetupClient()
		{
			LobbyManager.ChangeConnectionCount -= OnConnectionCountChange;
			LobbyManager.ClientSetup -= SetupClient;
			gameObject.SetActive(false);
		}

		void OnConnectionCountChange(int count)
		{
			_playerCountText.text = $"Player Count: {count}";

			if (!_isHost)
				return;

			if (_lobby.ConnectionCount == 3)
				_startGameButton.SetActive(true);
			else
				_startGameButton.SetActive(false);
		}
	}
}
