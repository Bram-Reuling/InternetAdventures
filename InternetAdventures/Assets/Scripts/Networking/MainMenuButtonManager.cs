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
            Debug.Log("Joining lobby");
            EventBroker.CallJoinLobbyEvent();
        }

        public void InvokeHostLobbyEvent()
        {
            Debug.Log("Hosting lobby");
            EventBroker.CallHostLobbyEvent();
        }
    }
}