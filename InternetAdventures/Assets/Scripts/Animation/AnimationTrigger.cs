using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator[] animation;
    
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
