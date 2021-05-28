using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementData : MonoBehaviour
{
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    public Vector3 MovementVector { get; private set; }
    public Quaternion DeltaRotation { get; private set; }

    private void Update()
    {
        MovementVector = (transform.position - _lastPosition);
        _lastPosition = transform.position;
        DeltaRotation = transform.rotation * Quaternion.Inverse(_lastRotation);
        _lastRotation = transform.rotation;
    }
}
