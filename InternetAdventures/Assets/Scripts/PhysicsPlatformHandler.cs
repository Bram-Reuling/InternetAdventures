using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsPlatformHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform1, platform2;
    private GameObject _actuatedPlatform;
    private Vector3 _endPosition = Vector3.zero;

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
        Debug.Log("Actuation stopped");
        platform1.GetComponent<Rigidbody>().DOKill();
        platform2.GetComponent<Rigidbody>().DOKill();
    }
}
