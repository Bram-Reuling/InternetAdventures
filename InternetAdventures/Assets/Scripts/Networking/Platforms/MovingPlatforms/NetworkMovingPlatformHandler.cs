using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using Mirror;
using UnityEngine;

namespace Networking.Platforms.MovingPlatforms
{
    public class NetworkMovingPlatformHandler : NetworkBehaviour
    {
        #region Variables

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
        public SyncList<GameObject> platforms = new SyncList<GameObject>();

        #endregion

        #region Global Functions

        

        #endregion

        #region Client Functions

        

        #endregion

        #region Server Functions

        [ServerCallback]
        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                _platformStations.Add(transform.GetChild(i).transform);
            }

            //Note: (transform.childCount * duration) / spawnRate gives the total amount of platforms that can be spawned
            //properly.
            _maxPlatformAmount = (int) (transform.childCount * duration / spawnRate);

            SpawnPlatform();
        }

        [ServerCallback]
        private void Update()
        {
            _timePassed += Time.deltaTime;
            if (!(_timePassed >= spawnRate)) return;
            
            if (loopMovement)
                if (_currentPlatformAmount >= _maxPlatformAmount)
                    return;
            SpawnPlatform();
        }

        [ServerCallback]
        private void SpawnPlatform()
        {
            //Todo: Create a deep copy of the list, so individual scripts cannot mess up the list.
            //LeanPool.Spawn(platform, _platformStations.ElementAt(0).position, Quaternion.identity, transform)
                //.GetComponent<NetworkMovingPlatform>()
                //.Initialize(_platformStations, duration, loopMovement);

            GameObject platformGameObject = LeanPool.Spawn(platform, _platformStations.ElementAt(0).position, Quaternion.identity, transform);
            platformGameObject.GetComponent<NetworkMovingPlatform>().Initialize(_platformStations, duration, loopMovement, this);
            
            NetworkServer.Spawn(platformGameObject);
            
            platforms.Add(platformGameObject);
            
            _timePassed = 0;
            if (useRandomSpawnIntervals)
                spawnRate = Random.Range(minSpawnTime, maxSpawnTime);
            _currentPlatformAmount++;
        }

        #endregion
    }
}