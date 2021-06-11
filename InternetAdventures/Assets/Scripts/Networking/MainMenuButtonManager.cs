using UnityEngine;

namespace Networking
{
    public class MainMenuButtonManager : MonoBehaviour
    {
        public void InvokeConnectToServerEvent()
        {
            EventBroker.CallConnectToServerEvent();
        }

        public void InvokeJoinLobbyEvent()
        {
            EventBroker.CallJoinLobbyEvent();
        }

        public void InvokeHostLobbyEvent()
        {
            EventBroker.CallHostLobbyEvent();
        }
    }
}