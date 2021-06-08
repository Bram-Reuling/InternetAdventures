using System;
using Shared.protocol.Lobby;
using TMPro;
using UnityEngine;

namespace Networking
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomCodeText;
        
        private void Start()
        {
            _roomCodeText.enabled = false;
            EventBroker.UpdateLobbyDataEvent += UpdateLobbyData;
        }

        private void UpdateLobbyData(LobbyDataResponse pLobbyDataResponse)
        {
            _roomCodeText.text = $"Code: {pLobbyDataResponse.Lobby.RoomCode}";
            _roomCodeText.enabled = true;
        }
    }
}