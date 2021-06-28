using Mirror;
using TMPro;
using UnityEngine;

namespace Networking
{
    public class PlayerName : NetworkBehaviour
    {
        [SerializeField, SyncVar(hook = nameof(SyncPlayerName))] private string playerName = "";
        [SerializeField] private TMP_Text playerNameText;

        [ServerCallback]
        public void ChangePlayerName(string pValue)
        {
            playerName = pValue;
            playerNameText.text = pValue;
        }

        [ClientCallback]
        private void SyncPlayerName(string oldName, string newName)
        {
            playerNameText.text = newName;
        }
    }
}