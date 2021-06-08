using System;
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
    }
}