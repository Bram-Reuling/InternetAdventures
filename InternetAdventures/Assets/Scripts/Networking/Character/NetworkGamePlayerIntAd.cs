using Mirror;
using UnityEngine;

namespace Networking.Character
{
    public class NetworkGamePlayerIntAd : NetworkBehaviour
    {
        [SyncVar] public string DisplayName = "Loading...";
        
        private InternetAdventuresNetworkManager room;

        private InternetAdventuresNetworkManager Room
        {
            get
            {
                if (room != null)
                {
                    return room;
                }

                return room = NetworkManager.singleton as InternetAdventuresNetworkManager;
            }
        }

        public override void OnStartClient()
        {
            Room.GamePlayers.Add(this);
            CmdAddPlayerToList();
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
            CmdRemovePlayerFromList();
        }
        
        [Command]
        private void CmdAddPlayerToList()
        {
            Room.GamePlayers.Add(this);
        }

        [Command]
        private void CmdRemovePlayerFromList()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }
    }
}