using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockwaveGun : Interactable
{
    [SerializeField] private float range;
    [SerializeField] private float shockwaveStrength;
    [SerializeField] private float shockwaveRadius;
    [SerializeField] private float possibleHitRadius;
    [SerializeField] private bool drawDebugInfo;
    [SerializeField] private LayerMask interactableLayers;
    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").performed += ShootGun;
    }

    private void Update()
    {
        if (drawDebugInfo) DrawDebug();
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        RaycastHit[] overlapColliders =
            Physics.SphereCastAll(transform.position, possibleHitRadius, transform.forward, range, interactableLayers);

        if (overlapColliders.Length > 0)
        {
            Vector3 hitPositon = overlapColliders[0].point;

            Collider[] collidersInRange = Physics.OverlapSphere(hitPositon, shockwaveRadius);
            foreach (var collider in collidersInRange)
            {
                if (collider.TryGetComponent(typeof(Rigidbody), out var rigidbody))
                {
                    ((Rigidbody) rigidbody).AddExplosionForce(shockwaveStrength, hitPositon, shockwaveRadius);
                }
            }
        }

        //Deprecated - uses only ray

        // var raycastHit = new RaycastHit();
        // if (Physics.Raycast(transform.position, transform.forward, out raycastHit, range))
        // {
        //     Vector3 hitPosition = raycastHit.point;
        //     Collider[] collidersInRange = Physics.OverlapSphere(hitPosition, shockwaveRadius);
        //     foreach (var objectInCollider in collidersInRange)
        //     {
        //         if (objectInCollider.TryGetComponent(typeof(Rigidbody), out var objectRigidbody))
        //         {
        //             ((Rigidbody)objectRigidbody).AddExplosionForce(shockwaveStrength, hitPosition, shockwaveRadius);
        //         }
        //     }
        // }
    }

    private void DrawDebug()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
    }
}