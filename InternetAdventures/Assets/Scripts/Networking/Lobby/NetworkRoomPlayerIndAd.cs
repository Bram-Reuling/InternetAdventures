using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.Lobby
{
    public class NetworkRoomPlayerIndAd : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];
        [SerializeField] private Button startGameButton = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private bool isLeader;

        public bool IsLeader
        {
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);
                RpcEnableGameButton(value);
                Debug.Log("Enabled Start button");
            }
        }

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

        [TargetRpc]
        private void RpcEnableGameButton(bool pValue)
        {
            startGameButton.gameObject.SetActive(pValue);
        }

        [Command]
        private void CmdAddPlayerToList()
        {
            Room.RoomPlayers.Add(this);
        }

        [Command]
        private void CmdRemovePlayerFromList()
        {
            Room.RoomPlayers.Remove(this);
        }
        
        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
            
            lobbyUI.SetActive(true);
        }

        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);
            CmdAddPlayerToList();
            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);
            CmdRemovePlayerFromList();
            UpdateDisplay();
        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (!hasAuthority)
            {
                foreach (var player in Room.RoomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }
                
                return;
            }

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting for Player...";
                playerReadyTexts[i].text = string.Empty;
            }

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
                playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady
                    ? "<color=green>Ready</color>"
                    : "<color=red>Not Ready</color>";
            }
        }

        public void HandleReadyToStart(bool readyStart)
        {
            if (!isLeader)
            {
                return;
            }

            startGameButton.interactable = readyStart;
            RpcSetButtonInteractable(readyStart);
        }

        [TargetRpc]
        private void RpcSetButtonInteractable(bool pValue)
        {
            startGameButton.interactable = pValue;
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {
            IsReady = !IsReady;
            
            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) {return;}
            Room.StartGame();
        }
    }
}   