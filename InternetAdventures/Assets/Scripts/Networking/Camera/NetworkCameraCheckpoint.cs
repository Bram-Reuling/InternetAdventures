using Mirror;
using UnityEngine;

namespace GameCamera
{
    public class NetworkCameraCheckpoint : NetworkBehaviour
    {
        private CameraRig cameraRig;
        [SerializeField] private Vector3 newCameraPosition;
        [SerializeField] private Vector3 newCameraRotation;
        [SerializeField] private float timeToChange;
        private bool canWalkThrough = true;

        [ClientCallback]
        private void Start()
        {
            cameraRig = FindObjectOfType<CameraRig>();
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Character") && canWalkThrough)
            {
                NetworkIdentity characterIdentity = other.gameObject.GetComponent<NetworkIdentity>();
                RpcOnTriggerEnter(characterIdentity.connectionToClient);
            }
        }

        [TargetRpc]
        private void RpcOnTriggerEnter(NetworkConnection connection)
        {
            cameraRig.ChangePerspective(newCameraPosition, newCameraRotation, timeToChange);
            canWalkThrough = false;
        }
    }
}