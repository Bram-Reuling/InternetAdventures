using UnityEngine;
using Lean.Pool;

public class MovingPlatformInfo : MonoBehaviour
{
    public Vector3 Movement { get; private set; }
    private Vector3 _finish;

    public void Initialize(in Vector3 pMovement, in Vector3 pFinish)
    {
        Movement = pMovement;
        _finish = pFinish;
    }
    
    private void Update()
    {
        transform.Translate(Movement * Time.deltaTime, Space.World);
        if((_finish - transform.position).magnitude < 0.1f) LeanPool.Despawn(gameObject);
    }
}
