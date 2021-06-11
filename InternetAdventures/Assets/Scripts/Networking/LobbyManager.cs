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
        [SerializeField] private GameObject _matchMakingButtonGameObject;
        [SerializeField] private Button _matchMakingButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _leaveButton;

        [SerializeField] private MainServerClient MainServerClient;

        private void Start()
        {
            _roomCodeText.enabled = false;
            EventBroker.UpdateLobbyDataEvent += UpdateLobbyData;
            
            EventBroker.CallLoadedLobbyPanelEvent();

            MainServerClient = FindObjectOfType<MainServerClient>();

            _matchMakingButton.onClick.AddListener(OnMatchButtonClick);
            _readyButton.onClick.AddListener(OnReadyButton);
            _leaveButton.onClick.AddListener(OnLeaveButton);
        }

        private void OnMatchButtonClick()
        {
            MainServerClient.StartMatch();
        }

        private void OnReadyButton()
        {
            MainServerClient.ChangeReadyState();
        }

        private void OnLeaveButton()
        {
            MainServerClient.LeaveLobby();
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
                _matchMakingButtonGameObject.SetActive(pLobbyDataResponse.Lobby.IsMatchmakingAllowed);   
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