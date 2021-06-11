using System;
using System.Collections;
using UnityEngine;

namespace Networking
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _joinHostPanel;
        [SerializeField] private GameObject _lobbyPanel;

        private void Start()
        {
            EventBroker.ChangePanelEvent += ChangePanelEvent;
        }

        private void ChangePanelEvent(string pPanel)    
        {
            switch (pPanel)
            {
                case "MainPanel":
                    Debug.Log("Enabling Main Panel!");
                    EnableMainPanel();
                    break;
                case "LobbyPanel":
                    Debug.Log("Enabling Lobby Panel");
                    EnableLobbyPanel();
                    break;
                case "JoinHostPanel":
                    Debug.Log("Enabling Host Join Panel");
                    EnableHostJoinPanel();
                    break;
                case "LobbyPanelFromGame":
                    Debug.Log("Enabling Lobby Panel");
                    EnableLobbyPanelFromGame();
                    break;
                default:
                    break;
            }
        }

        private void EnableMainPanel()
        {
            _loginPanel.SetActive(false);
            _mainPanel.SetActive(true);
        }

        private void EnableLobbyPanel()
        {
            _joinHostPanel.SetActive(false);
            _lobbyPanel.SetActive(true);
            
            EventBroker.CallLoadedLobbyPanelEvent();
        }

        private void EnableHostJoinPanel()
        {
            _lobbyPanel.SetActive(false);
            _joinHostPanel.SetActive(true);
        }

        private void EnableLobbyPanelFromGame()
        {
            if (_loginPanel == null)
            {
                Debug.Log("Login panel is null");
            }
            
            if (_mainPanel == null)
            {
                Debug.Log("Main panel is null");
            }
            
            if (_joinHostPanel == null)
            {
                Debug.Log("Join Host panel is null");
            }
            
            if (_lobbyPanel == null)
            {
                Debug.Log("Lobby panel is null");
            }
            _loginPanel.SetActive(false);
            _mainPanel.SetActive(true);
            _joinHostPanel.SetActive(false);
            _lobbyPanel.SetActive(true);
            EventBroker.CallLoadedLobbyPanelEvent();
        }
    }
}