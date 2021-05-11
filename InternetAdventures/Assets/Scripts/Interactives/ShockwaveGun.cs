using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockwaveGun : Interactable
{
    [SerializeField] private float range;
    [SerializeField] private float shockwaveStrength;
    [SerializeField] private float shockwaveRadius;
    private PlayerInput _playerInput;
    private void Start()
    {
        _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").performed += ShootGun;
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        var raycastHit = new RaycastHit();
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, range))
        {
            Vector3 hitPosition = raycastHit.point;
            Collider[] collidersInRange = Physics.OverlapSphere(hitPosition, shockwaveRadius);
            foreach (var objectInCollider in collidersInRange)
            {
                if (objectInCollider.TryGetComponent(typeof(Rigidbody), out var objectRigidbody))
                {
                    ((Rigidbody)objectRigidbody).AddExplosionForce(shockwaveStrength, hitPosition, shockwaveRadius);
                }
            }
        }
    }
}