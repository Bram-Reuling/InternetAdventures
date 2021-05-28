using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PressurePlateHandler : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject platformToMove;
    [SerializeField] private float movementDuration;
    [SerializeField] private bool loop;
    
    private readonly List<GameObject> _gameObjectsOnPressurePlate = new List<GameObject>();
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Start()
    {
        _initialPosition = platformToMove.transform.position;
        _initialRotation = platformToMove.transform.rotation;
    }

    public void AddGameObject(in GameObject pGameObject)
    {
        if (!_gameObjectsOnPressurePlate.Contains(pGameObject))
        {
          _gameObjectsOnPressurePlate.Add(pGameObject);
          if (_gameObjectsOnPressurePlate.Count == 1)
          {
              platformToMove.transform.DOMove(targetTransform.position, movementDuration);
              platformToMove.transform.DORotateQuaternion(targetTransform.rotation, movementDuration);
          }
        }
    }

    public void RemoveGameObject(in GameObject pGameObject)
    {
        if (_gameObjectsOnPressurePlate.Contains(pGameObject))
        {
            _gameObjectsOnPressurePlate.Remove(pGameObject);
            if (_gameObjectsOnPressurePlate.Count == 0)
            {
                DOTween.Kill(platformToMove);
                platformToMove.transform.DOMove(_initialPosition, movementDuration);
                platformToMove.transform.DORotateQuaternion(_initialRotation, movementDuration);
            }
        }
    }
}
