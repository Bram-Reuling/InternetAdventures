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

        [SyncVar(hook = nameof(SyncGoodCount))] private int goodMembers = 0;
        [SyncVar(hook = nameof(SyncBadCount))] private int badMembers = 0;

        [ClientCallback]
        private void SyncGoodCount(int oldCount, int newCount)
        {
            goodMembers = newCount;
            spGoodCommText.text = $"Good Community Members: {goodMembers}";
        }
        
        [ClientCallback]
        private void SyncBadCount(int oldCount, int newCount)
        {
            badMembers = newCount;
            spBadCommText.text = $"Bad Community Members: {badMembers}";
        }
        
        [ServerCallback]
        private void Start()
        {
            //EventBroker.LoseWinEvent += LoseWinEvent;
            geEndGameButton.onClick.AddListener(EventBroker.CallMatchEndEvent);
            
            NetworkLoseWinHandler.OnNoGoodMembers += NetworkLoseWinHandlerOnOnNoGoodMembers;
            NetworkLoseWinHandler.OnNoBadMembers += NetworkLoseWinHandlerOnOnNoBadMembers;

            badMembers = NetworkLoseWinHandler.badCommunityMembers.Count;
            goodMembers = NetworkLoseWinHandler.goodCommunityMembers.Count;
        }

        [ServerCallback]
        private void NetworkLoseWinHandlerOnOnNoBadMembers(object sender, EventArgs e)
        {
            LoseWinEvent("Win");
        }

        [ServerCallback]
        private void NetworkLoseWinHandlerOnOnNoGoodMembers(object sender, EventArgs e)
        {
            LoseWinEvent("Lost");
        }

        [ServerCallback]
        private void LoseWinEvent(string pValue)
        {
            Debug.LogError("Switching panels!");
            statPanel.SetActive(false);

            geWinLoseText.text = $"You {pValue}!";
            geBadCommText.text = $"Bad Members Left: {badMembers}";
            geGoodCommText.text = $"Good Members Left: {goodMembers}";
            
            gameEndPanel.SetActive(true);
            RpcLoseWinEvent(pValue);
        }

        [ClientRpc]
        private void RpcLoseWinEvent(string pValue)
        {
            Debug.LogError("Switching panels!");
            statPanel.SetActive(false);

            geWinLoseText.text = $"You {pValue}!";
            geBadCommText.text = $"Bad Members Left: {badMembers}";
            geGoodCommText.text = $"Good Members Left: {goodMembers}";
            
            gameEndPanel.SetActive(true);
        }
        
        private void Update()
        {
            spBadCommText.text = $"Bad Community Members: {badMembers}";
            spGoodCommText.text = $"Good Community Members: {goodMembers}";
            ServerUpdate();
        }

        [ServerCallback]
        private void ServerUpdate()
        {
            badMembers = NetworkLoseWinHandler.badCommunityMembers.Count;
            goodMembers = NetworkLoseWinHandler.goodCommunityMembers.Count;
        }

        [ServerCallback]
        private void OnDestroy()
        {
            EventBroker.LoseWinEvent -= LoseWinEvent;
        }
    }
}