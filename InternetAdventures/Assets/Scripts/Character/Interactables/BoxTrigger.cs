using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hands;
    [SerializeField] private GameObject banHammer;
    private Hands _hands;
    private BanHammer _banHammer;

    private void Start()
    {
        _hands = hands.GetComponent<Hands>();
        _banHammer = banHammer.GetComponent<BanHammer>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (hands.activeSelf)
        {
            _hands.AddOnTriggerEnter(other);
        }
        else if(banHammer.activeSelf)
        {
            _banHammer.AddOnTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
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
