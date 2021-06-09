using System;
using System.Linq;
using Shared.model;
using Shared.protocol.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomCodeText;
        [SerializeField] private TMP_Text _playerOneText;
        [SerializeField] private TMP_Text _playerTwoText;
        [SerializeField] private GameObject _matchMakingButton;
        
        private void Start()
        {
            _roomCodeText.enabled = false;
            EventBroker.UpdateLobbyDataEvent += UpdateLobbyData;
        }

        private void UpdateLobbyData(LobbyDataResponse pLobbyDataResponse, int pClientId)
        {
            _roomCodeText.text = $"Code: {pLobbyDataResponse.Lobby.RoomCode}";
            _roomCodeText.enabled = true;

            if (pLobbyDataResponse.Lobby.Players.Count == 2)
            {
                _playerOneText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[0]);
                _playerTwoText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[1]);
            }
            else if (pLobbyDataResponse.Lobby.Players.Count == 1)
            {
                _playerOneText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[0]);
                _playerTwoText.text = "Waiting for player..";
            }

            Client thisClient = pLobbyDataResponse.Lobby.Players.FirstOrDefault(c => c.Id == pClientId);

            if (thisClient.IsLobbyLeader)
            {
                _matchMakingButton.SetActive(pLobbyDataResponse.Lobby.IsMatchmakingAllowed);   
            }
        }

        private string CheckReadyStateOfPlayer(Client player)
        {
            return player.ReadyState switch
            {
                ReadyState.Ready => $"<color=green>{player.Name}</color>",
                ReadyState.NotReady => $"<color=red>{player.Name}</color>",
                _ => $"{player.Name}"
            };
        }   
    }
}