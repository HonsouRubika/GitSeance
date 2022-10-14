using FishNet.Object;
using Seance.Networking;

namespace Seance.Player
{
    public class PlayerInstance : NetworkBehaviour
    {
        LobbyManager _lobbyManager;

        #region Connection to server

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
                return;

            _lobbyManager = FindObjectOfType<LobbyManager>();
            LobbyManager.OnClientSetup += SetupClient;
            _lobbyManager._ownedConnection = LocalConnection;
            _lobbyManager._ownedPlayerInstance = this;
            _lobbyManager.ServerAddConnection(LocalConnection);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!IsOwner)
                return;

            _lobbyManager.ServerRemoveConnection(LocalConnection);
        }

        void SetupClient()
        {
            for (int i = 0; i < _lobbyManager._connections.Count; i++)
            {
                if (_lobbyManager._connections[i].ClientId == _lobbyManager._ownedConnection.ClientId)
                {
                    _lobbyManager._ownedConnectionReferencePosition = i;
                    break;
                }
            }
        }

        #endregion
    }
}
