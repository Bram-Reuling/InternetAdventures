using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class GameUIManager : NetworkBehaviour
    {
        [SerializeField] private GameObject statPanel;
        [SerializeField] private TMP_Text spBadCommText;
        [SerializeField] private TMP_Text spGoodCommText;
        
        [SerializeField] private GameObject gameEndPanel;
        [SerializeField] private TMP_Text geWinLoseText;
        [SerializeField] private TMP_Text geBadCommText;
        [SerializeField] private TMP_Text geGoodCommText;
        [SerializeField] private Button geEndGameButton;

        private int goodMembers = 0;
        private int badMembers = 0;

        [ClientCallback]
        private void Start()
        {
            EventBroker.LoseWinEvent += LoseWinEvent;
            EventBroker.ChangeMembersCount += ChangeMembersCount;
            geEndGameButton.onClick.AddListener(EventBroker.CallMatchEndEvent);
        }

        [ClientCallback]
        private void ChangeMembersCount(int pIntValue, string pStringValue)
        {
            switch (pStringValue)
            {
                case "good":
                    goodMembers = pIntValue;
                    break;
                case "bad":
                    badMembers = pIntValue;
                    break;
                default:
                    break;
            }
        }

        [ClientCallback]
        private void LoseWinEvent(string pValue)
        {
            Debug.LogError("Switching panels!");
            statPanel.SetActive(false);

            geWinLoseText.text = $"You {pValue}!";
            geBadCommText.text = $"Bad Members Left: {badMembers}";
            geGoodCommText.text = $"Good Members Left: {goodMembers}";
            
            gameEndPanel.SetActive(true);

        }

        [ClientCallback]
        private void Update()
        {
            spBadCommText.text = $"Bad Community Members: {badMembers}";
            spGoodCommText.text = $"Good Community Members: {goodMembers}";
        }

        [ClientCallback]
        private void OnDestroy()
        {
            EventBroker.LoseWinEvent -= LoseWinEvent;
            EventBroker.ChangeMembersCount -= ChangeMembersCount;
        }
    }
}