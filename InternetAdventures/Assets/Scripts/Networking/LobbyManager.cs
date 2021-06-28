using System;
using System.Collections;
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
        [SerializeField] private TMP_Text _playerOneReadyText;
        [SerializeField] private TMP_Text _playerTwoReadyText;
        [SerializeField] private GameObject _matchMakingButtonGameObject;
        [SerializeField] private Button _matchMakingButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _leaveButton;

        [SerializeField] private MainServerClient MainServerClient;

        private void Start()
        {
            _roomCodeText.enabled = false;
            EventBroker.UpdateLobbyDataEvent += UpdateLobbyData;

            MainServerClient = FindObjectOfType<MainServerClient>();

            _matchMakingButton.onClick.AddListener(OnMatchButtonClick);
            _readyButton.onClick.AddListener(OnReadyButton);
            _leaveButton.onClick.AddListener(OnLeaveButton);

            StartCoroutine(LobbyLoaded());
        }

        IEnumerator LobbyLoaded()
        {
            yield return new WaitForSeconds(0.1f);
            EventBroker.CallLoadedLobbyPanelEvent();
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
            if (_roomCodeText == null)
            {
                Debug.Log("code text is null");
                return;
            }
            
            if (_playerOneText == null)
            {
                Debug.Log("p1 text is null");
                return;
            }
            
            if (_playerTwoText == null)
            {
                Debug.Log("p2 text is null");
                return;
            }
            
            _roomCodeText.text = $"Room ID: {pLobbyDataResponse.Lobby.RoomCode}";
            _roomCodeText.enabled = true;

            if (pLobbyDataResponse.Lobby.Players.Count == 2)
            {
                _playerOneText.text = pLobbyDataResponse.Lobby.Players[0].Name;
                _playerTwoText.text = pLobbyDataResponse.Lobby.Players[1].Name;

                _playerOneReadyText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[0]);
                _playerTwoReadyText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[1]);
            }
            else if (pLobbyDataResponse.Lobby.Players.Count == 1)
            {
                _playerOneText.text = pLobbyDataResponse.Lobby.Players[0].Name;
                _playerOneReadyText.text = CheckReadyStateOfPlayer(pLobbyDataResponse.Lobby.Players[0]);
                _playerTwoText.text = "Waiting for player..";
                _playerTwoReadyText.text = "";
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
                ReadyState.Ready => "<color=green>Ready</color>",
                ReadyState.NotReady => "<color=red>Not Ready</color>",
                _ => ""
            };
        }

        private void OnDestroy()
        {
            EventBroker.UpdateLobbyDataEvent -= UpdateLobbyData;
        }
    }
}