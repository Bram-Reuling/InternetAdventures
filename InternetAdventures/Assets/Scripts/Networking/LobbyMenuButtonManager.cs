using UnityEngine;

namespace Networking
{
    public class LobbyMenuButtonManager : MonoBehaviour
    {
        public void InvokeReadyEvent()
        {
            EventBroker.CallReadyEvent();
        }

        public void InvokeLeaveEvent()
        {
            EventBroker.CallLeaveEvent();
        }

        public void InvokeStartMatchEvent()
        {
            EventBroker.CallStartMatchEvent();
        }
    }
}