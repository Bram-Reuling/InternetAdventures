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
                case "JoinHostPanel":
                    Debug.Log("Enabling Host Join Panel");
                    EnableHostJoinPanel();
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

        private void EnableHostJoinPanel()
        {
            _joinHostPanel.SetActive(true);
        }
    }
}