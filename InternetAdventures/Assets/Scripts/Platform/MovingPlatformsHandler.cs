using UnityEngine;
using Lean.Pool;

public class MovingPlatformsHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private float duration;
    [SerializeField] private float spawnRate;
    [SerializeField] private bool useRandomSpawnIntervals;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    
    private float timePassed;
    private Transform From, To;

    private void Start()
    {
        From = transform.GetChild(0);
        To = transform.GetChild(1);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= spawnRate)
        {
            LeanPool.Spawn(platform, To.position, Quaternion.identity, transform).GetComponent<MovingPlatform>()
                .Initialize(To.position, duration);
            timePassed = 0;
            if(useRandomSpawnIntervals) 
                spawnRate = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
}
