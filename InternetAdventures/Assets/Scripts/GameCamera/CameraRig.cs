using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameCamera
{
    public class CameraRig : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private List<GameObject> targets;
        
        [SerializeField] private bool canMoveOnXAxis;
        [SerializeField] private bool canMoveOnYAxis;
        [SerializeField] private bool canMoveOnZAxis;

        private void Start()
        {
            if (camera == null)
            {
                Debug.LogError("NO CAMERA HAS BEEN FOUND");
            }

            transform.position = DetermineTargetPosition(new Vector3());
        }

        private void Update()
        {
            Vector3 targetPosition = new Vector3();

            targetPosition = DetermineTargetPosition(targetPosition);

            FollowTarget(targetPosition);
        }

        private Vector3 DetermineTargetPosition(Vector3 targetPosition)
        {
            if (targets.Count > 1)
            {
                foreach (GameObject target in targets)
                {
                    targetPosition += target.gameObject.transform.position;
                }

                targetPosition /= targets.Count;
            }
            else if (targets.Count == 1)
            {
                targetPosition = targets[0].gameObject.transform.position;
            }
            else
            {
                targetPosition = Vector3.zero;
            }

            return targetPosition;
        }

        private void FollowTarget(Vector3 pTargetPosition)
        {
            Vector3 newPosition = transform.position;
            Vector3 targetPosition = pTargetPosition;
            Vector3 rigPosition = transform.position;

            if (canMoveOnXAxis)
            {
                newPosition.x = FollowAxisPosition(targetPosition.x, rigPosition.x);
            }

            if (canMoveOnYAxis)
            {
                newPosition.y = FollowAxisPosition(targetPosition.y, rigPosition.y);
            }

            if (canMoveOnZAxis)
            {
                newPosition.z = FollowAxisPosition(targetPosition.z, rigPosition.z);    
            }

            transform.position = newPosition;
            
            camera.transform.LookAt(pTargetPosition);
        }

        private float FollowAxisPosition(float pTargetAxisValue, float pRigAxisValue)
        {
            return pRigAxisValue + pTargetAxisValue - pRigAxisValue;
        }
    }
}
