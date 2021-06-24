using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class ShowWinLoss : NetworkBehaviour
    {
        [ServerCallback]
        private void Start()
        {
            NetworkLoseWinHandler.OnNoGoodMembers += NetworkLoseWinHandlerOnOnNoGoodMembers;
            NetworkLoseWinHandler.OnNoBadMembers += NetworkLoseWinHandlerOnOnNoBadMembers;
        }
        
        [ServerCallback]
        private void NetworkLoseWinHandlerOnOnNoBadMembers(object sender, EventArgs e)
        {
            RpcShowDebug("Won");
        }

        [ClientRpc]
        private void RpcShowDebug(string pValue)
        {
            Debug.LogError(pValue);
        }

        [ServerCallback]
        private void NetworkLoseWinHandlerOnOnNoGoodMembers(object sender, EventArgs e)
        {
            RpcShowDebug("Lost");
        }
    }
}