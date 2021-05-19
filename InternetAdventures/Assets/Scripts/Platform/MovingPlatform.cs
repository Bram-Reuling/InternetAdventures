using DG.Tweening;
using UnityEngine;
using Lean.Pool;

public class MovingPlatform : MonoBehaviour
{
    private float _duration;
    private Vector3 _finish;

    public void Initialize(in Vector3 pFinish, in float pDuration)
    {
        _duration = pDuration;
        _finish = pFinish;
        transform.DOMove(_finish, _duration).SetEase(Ease.Linear);
    }
    
    private void Update()
    {
        //Deprecated
        //transform.Translate(Movement * Time.deltaTime, Space.World);
        //if((_finish - transform.position).magnitude < 0.1f) LeanPool.Despawn(gameObject);
        
        //New
        if(!DOTween.IsTweening(transform)) LeanPool.Despawn(gameObject);
    }
}
