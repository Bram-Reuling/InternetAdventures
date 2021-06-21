using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animation animation;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            animation.Play();
            Destroy(gameObject);
        }
    }
}
