using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Networking.Platforms.PhysicsPlatform
{
    public class NetworkPhysicsPlatform : NetworkBehaviour
    {
        #region Variables

        [SerializeField] private NetworkPhysicsPlatformHandler _physicsPlatformHandler;
        private readonly Dictionary<GameObject, float> _platformGameObjects = new Dictionary<GameObject, float>();
        private float _totalMass;

        #endregion

        #region Server Functions

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (_platformGameObjects.ContainsKey(other.gameObject)) return;
            if ((LayerMask.GetMask("Moveable") & (1 << other.gameObject.layer)) <= 0) return;
            AddGameObject(other.gameObject);
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if ((LayerMask.GetMask("Moveable") & (1 << other.gameObject.layer)) <= 0) return;
            RemoveGameObject(other.gameObject);
        }

        [ServerCallback]
        public float GetTotalMass()
        {
            return _totalMass;
        }

        [ServerCallback]
        public void AddCharacter(GameObject pCharacter)
        {
            AddGameObject(pCharacter);
        }

        [ServerCallback]
        public void RemoveCharacter(GameObject pCharacter)
        {
            RemoveGameObject(pCharacter);
        }

        [ServerCallback]
        private void AddGameObject(GameObject pGameObject)
        {
            float currentMass = 0;
            if (pGameObject.CompareTag("Character"))
                currentMass = pGameObject.GetComponent<NetworkCharacterMovement>().CharacterMass;
            else if ((LayerMask.GetMask("Moveable") & (1 << pGameObject.gameObject.layer)) > 0)
                currentMass = pGameObject.GetComponent<Rigidbody>().mass;
            if (!_platformGameObjects.ContainsKey(pGameObject))
                _platformGameObjects.Add(pGameObject, currentMass);
            _totalMass += currentMass;
            _physicsPlatformHandler.OnActuation();
        }

        [ServerCallback]
        private void RemoveGameObject(GameObject pGameObject)
        {
            if (!_platformGameObjects.ContainsKey(pGameObject)) return;
            _totalMass -= _platformGameObjects[pGameObject];
            _platformGameObjects.Remove(pGameObject);
            _physicsPlatformHandler.OnActuation();
        }

        #endregion
    }
}