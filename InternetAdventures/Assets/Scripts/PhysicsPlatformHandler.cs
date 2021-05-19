using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhysicsPlatformHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform1, platform2;
    [SerializeField] private bool movePlatformsBack;
    [SerializeField] private float movementDuration;
    private GameObject _actuatedPlatform;
    private Vector3 _endPosition = Vector3.zero;
    private Vector3 _initialMovementVector;

    private Vector3 _platform1InitPosition;
    private Vector3 _platform2InitPosition;

    private void Start()
    {
        //Save initial position to check difference when moving back.
        _platform1InitPosition = platform1.transform.position;
        _platform2InitPosition = platform2.transform.position;
        
        //Calculate movement vector by taking the higher platform.
        Transform platformToUse = platform1.transform.position.y >= platform2.transform.position.y
            ? platform1.transform
            : platform2.transform;
        if (Physics.Raycast(transform.TransformDirection(platformToUse.position), Vector3.down, out var hit,
            float.PositiveInfinity))
        {
            _initialMovementVector = hit.point - transform.TransformDirection(platform2.transform.position);
        }
        else
        {
            Debug.LogError("Could not calculate the movement vector of the physics platform!");
        }
    }
    
    
    public void OnActuation(GameObject pPlatform, GameObject pActuator)
    {
        if (pPlatform == _actuatedPlatform) return;
        
        Vector3 platformToUser = pActuator.transform.position - pPlatform.transform.position;
        if (Vector3.Dot(platformToUser.normalized, Vector3.up) < 0.2f) return;
        
        _actuatedPlatform = pPlatform;
        Vector3 upwardsVector = Vector3.zero;

        if (Physics.Raycast(_actuatedPlatform.transform.position, Vector3.down, out var hit, float.PositiveInfinity))
        {
            _endPosition = hit.point;
            upwardsVector = -(_endPosition - _actuatedPlatform.transform.position);
        }
        else return;
        
        _actuatedPlatform.GetComponent<Rigidbody>().DOMoveY(_endPosition.y, 2);
        if (_actuatedPlatform == platform1)
            platform2.GetComponent<Rigidbody>().DOMoveY((platform2.transform.position + upwardsVector).y, 2);
        else
            platform1.GetComponent<Rigidbody>().DOMoveY((platform1.transform.position + upwardsVector).y, 2);
    }

    public void StopActuation()
    {
        if (movePlatformsBack)
            MovePlatformsBack();
    }
    
    private void MovePlatformsBack()
    {
        platform1.GetComponent<Rigidbody>().DOMoveY(_platform1InitPosition.y, movementDuration);
        platform2.GetComponent<Rigidbody>().DOMoveY(_platform2InitPosition.y, movementDuration);
        _actuatedPlatform = null;
    }
}
