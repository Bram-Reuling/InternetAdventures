﻿using UnityEngine;

namespace Networking
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject[] _mainPanel;
        [SerializeField] private GameObject _joinHostPanel;

        [SerializeField] private GameObject _startButton;

        private void Start()
        {
            EventBroker.ChangePanelEvent += ChangePanelEvent;

            switch (DataHandler.MenuState)
            {
                case "LoginPanel":
                    break;
                case "MainPanel":
                    EnableMainPanel();
                    break;
                case "JoinHostPanel":
                    EnableHostJoinPanel();
                    break;
            }
        }

        private void ChangePanelEvent(string pPanel)    
        {
            switch (pPanel)
            {
                case "MainPanel":
                    Debug.Log("Enabling Main Panel!");
                    EnableMainPanel();
                    break;
                case "JoinHostPanel":
                    Debug.Log("Enabling Host Join Panel");
                    EnableMainPanel();
                    break;
                default:
                    break;
            }
        }

        private void EnableMainPanel()
        {
            _loginPanel.SetActive(false);

            foreach (GameObject o in _mainPanel)
            {
                o.SetActive(true);
            }

            _startButton.SetActive(false);
            
            DataHandler.MenuState = "MainPanel";
        }

        private void EnableHostJoinPanel()
        {
            //yield return new WaitForSeconds(0.05f);
            
            EnableMainPanel();
            _joinHostPanel.SetActive(true);

            DataHandler.MenuState = "JoinHostPanel";
        }

        private void OnDestroy()
        {
            EventBroker.ChangePanelEvent -= ChangePanelEvent;
        }
    }
}