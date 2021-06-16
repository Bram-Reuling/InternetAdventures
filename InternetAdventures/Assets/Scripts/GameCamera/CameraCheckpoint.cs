using System;
using UnityEngine;

namespace GameCamera
{
    public class CameraCheckpoint : MonoBehaviour
    {
        private CameraRig cameraRig;
        [SerializeField] private Vector3 cameraPositionOffset;
        [SerializeField] private Vector3 cameraRotationOffset;
        [SerializeField] private float timeToChange;
        private bool canWalkThrough = true;

        private void Start()
        {
            cameraRig = FindObjectOfType<CameraRig>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Character") && canWalkThrough)
            {
                Debug.Log("Test");
                cameraRig.ChangePerspective(cameraPositionOffset, cameraRotationOffset, timeToChange);
                canWalkThrough = false;
            }
        }
    }
}