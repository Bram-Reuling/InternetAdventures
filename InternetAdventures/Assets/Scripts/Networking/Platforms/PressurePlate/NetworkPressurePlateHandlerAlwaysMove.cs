using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class NetworkPressurePlateHandlerAlwaysMove : NetworkBehaviour
{
    #region Variables

    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject platformToMove;
    [SerializeField] private float movementDuration;
    [SerializeField] private bool loop;
    
    private readonly List<GameObject> _gameObjectsOnPressurePlate = new List<GameObject>();
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    #endregion

    #region Global Functions

    private void Start()
    {
        _initialPosition = platformToMove.transform.position;
        _initialRotation = platformToMove.transform.rotation;
    }

    #endregion

    #region Server Functions

    [ServerCallback]
    public void AddGameObject(in GameObject pGameObject)
    {
        if (_gameObjectsOnPressurePlate.Contains(pGameObject)) return;
        _gameObjectsOnPressurePlate.Add(pGameObject);
        if (_gameObjectsOnPressurePlate.Count == 1 && !loop)
            MoveToTransform(targetTransform);
    }

    [ServerCallback]
    public void RemoveGameObject(in GameObject pGameObject)
    {
        if (!_gameObjectsOnPressurePlate.Contains(pGameObject)) return;
        _gameObjectsOnPressurePlate.Remove(pGameObject);
        if (_gameObjectsOnPressurePlate.Count == 0)
            MoveToTransform(_initialPosition, _initialRotation);
    }
    
    [ServerCallback]
    private void Update()
    {
        //if (_gameObjectsOnPressurePlate.Count <= 0 || !loop) return;
        if (platformToMove.transform.position == _initialPosition && platformToMove.transform.rotation == _initialRotation)
            MoveToTransform(targetTransform);
        else if (platformToMove.transform.position == targetTransform.position && platformToMove.transform.rotation == targetTransform.rotation)
            MoveToTransform(_initialPosition, _initialRotation);
    }

    [ServerCallback]
    private void MoveToTransform(Vector3 pPosition, Quaternion pRotation)
    {
        platformToMove.transform.DOMove(pPosition, movementDuration);
        platformToMove.transform.DORotateQuaternion(pRotation, movementDuration);
    }
    
    [ServerCallback]
    private void MoveToTransform(Transform pTransform)
    {
        DOTween.Kill(platformToMove);
        platformToMove.transform.DOMove(pTransform.position, movementDuration);
        platformToMove.transform.DORotateQuaternion(pTransform.rotation, movementDuration);
    }

    #endregion

    #region Client Functions

    

    #endregion
}

