using Mirror;
using UnityEngine;

public class NetworkBoxTrigger : NetworkBehaviour
{
    [SerializeField] private GameObject hands;
    [SerializeField] private GameObject banHammer;
    private NetworkHands _hands;
    private NetworkBanHammer _banHammer;

    private void Start()
    {
        _hands = hands.GetComponent<NetworkHands>();
        _banHammer = banHammer.GetComponent<NetworkBanHammer>();
    }
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (_hands == null || _banHammer == null) return;
        if (hands.activeSelf)
        {
            _hands.AddOnTriggerEnter(other);
        }
        else if(banHammer.activeSelf)
        {
            _banHammer.AddOnTriggerEnter(other);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (_hands == null || _banHammer == null) return;
        if (hands.activeSelf)
        {
            _hands.RemoveOnTriggerExit(other);
        }
        else if (banHammer.activeSelf)
        {
            _banHammer.RemoveOnTriggerExit(other);
        }
    }
}
