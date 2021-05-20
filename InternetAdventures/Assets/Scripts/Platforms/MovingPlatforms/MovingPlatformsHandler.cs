using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;

public class MovingPlatformsHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private float duration;
    [SerializeField] private float spawnRate;
    [SerializeField] private bool useRandomSpawnIntervals;
    [SerializeField] private bool loopMovement;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    
    private float _timePassed;
    private int _maxPlatformAmount;
    private int _currentPlatformAmount;
    private readonly List<Transform> _platformStations = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _platformStations.Add(transform.GetChild(i).transform);
        }
        //Note: (transform.childCount * duration) / spawnRate gives the total amount of platforms that can be spawned
        //properly.
        _maxPlatformAmount = (int)(transform.childCount * duration / spawnRate);
        
        SpawnPlatform();
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed >= spawnRate)
        {
            if (loopMovement)
                if (_currentPlatformAmount >= _maxPlatformAmount) return;
            SpawnPlatform();
        }
    }

    private void SpawnPlatform()
    {
        //Todo: Create a deep copy of the list, so individual scripts cannot mess up the list.
        LeanPool.Spawn(platform, _platformStations.ElementAt(0).position, Quaternion.identity, transform).GetComponent<MovingPlatform>()
            .Initialize(_platformStations, duration, loopMovement);
        _timePassed = 0;
        if(useRandomSpawnIntervals) 
            spawnRate = Random.Range(minSpawnTime, maxSpawnTime);
        _currentPlatformAmount++;
    }
}
