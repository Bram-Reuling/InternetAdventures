using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsPlatform : MonoBehaviour
{
    [SerializeField] private PhysicsPlatformHandler _physicsPlatformHandler;
    private readonly Dictionary<GameObject, float> _platformGameObjects = new Dictionary<GameObject, float>();
    private float _totalMass;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_platformGameObjects.ContainsKey(other.gameObject)) return;
        if ((LayerMask.GetMask("Moveable") & (1 << other.gameObject.layer)) <= 0) return;
        Debug.Log(other.gameObject.name + " added");
        AddGameObject(other.gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if ((LayerMask.GetMask("Moveable") & (1 << other.gameObject.layer)) <= 0) return;
        Debug.Log(other.gameObject.name + " exited");
        RemoveGameObject(other.gameObject);
    }

    public float GetTotalMass()
    {
        return _totalMass;
    }

    public void AddCharacter(GameObject pCharacter)
    {
        AddGameObject(pCharacter);
    }

    public void RemoveCharacter(GameObject pCharacter)
    {
        RemoveGameObject(pCharacter);
    }

    private void AddGameObject(GameObject pGameObject)
    {
        float currentMass = 0;
        if (pGameObject.CompareTag("Character"))
            currentMass = pGameObject.GetComponent<CharacterMovement>().CharacterMass;
        else if((LayerMask.GetMask("Moveable") & (1 << pGameObject.gameObject.layer)) > 0)
            currentMass = pGameObject.GetComponent<Rigidbody>().mass;
        if(!_platformGameObjects.ContainsKey(pGameObject)) 
            _platformGameObjects.Add(pGameObject, currentMass);
        _totalMass += currentMass;
        _physicsPlatformHandler.OnActuation();
    }

    private void RemoveGameObject(GameObject pGameObject)
    {
        if (!_platformGameObjects.ContainsKey(pGameObject)) return;
        _totalMass -= _platformGameObjects[pGameObject];
        _platformGameObjects.Remove(pGameObject);
        _physicsPlatformHandler.OnActuation();
    }
}
