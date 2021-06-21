using Mirror;
using UnityEngine;

public class NetworkAnimationTrigger : NetworkBehaviour
{
    [SerializeField] private Animator[] animation;
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            foreach (var currentAnimation in animation)
            {
                currentAnimation.enabled = true;
            }
            Destroy(gameObject);
        }
    }
}
