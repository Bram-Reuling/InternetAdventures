using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class NetworkPositionAndRotation : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SyncPosition))] private Vector3 position;
        [SyncVar(hook = nameof(SyncRotation))] private Quaternion rotation;

        [ServerCallback]
        private void Update()
        {
            var objectTransform = transform;
            
            position = objectTransform.position;
            rotation = objectTransform.rotation;
        }

        [ClientCallback]
        private void SyncPosition(Vector3 lastPosition, Vector3 newPosition)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, 1);
        }

        [ClientCallback]
        private void SyncRotation(Quaternion lastRotation, Quaternion newRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 1);
        }
    }
}