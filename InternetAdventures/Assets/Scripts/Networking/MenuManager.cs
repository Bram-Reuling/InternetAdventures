﻿using System;
using UnityEngine;

namespace Networking
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _mainPanel;

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
                    break;
                default:
                    break;
            }
        }
    }
}