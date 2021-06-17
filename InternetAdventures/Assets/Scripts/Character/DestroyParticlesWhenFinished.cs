using System.Collections;
using UnityEngine;

public class DestroyParticlesWhenFinished : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    
    //Gets particle system on the same game object and destroys it after the particle system has finished playing.
    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(DestroyWhenFinished());
    }
    
    private IEnumerator DestroyWhenFinished()
    {
        yield return new WaitUntil(() => !_particleSystem.isPlaying);
        Destroy(gameObject);
    }
}
