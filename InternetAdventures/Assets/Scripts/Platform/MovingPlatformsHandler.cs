using UnityEngine;
using Lean.Pool;

public class MovingPlatformsHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float spawnRate;
    private float timePassed;
    private Transform From, To;
    private Vector3 _direction;

    private void Start()
    {
        From = transform.GetChild(0);
        To = transform.GetChild(1);
        _direction = To.position - From.position;
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= spawnRate)
        {
            LeanPool.Spawn(platform, From.position, Quaternion.identity, transform).GetComponent<MovingPlatformInfo>()
                .Initialize(_direction.normalized * movementSpeed, To.position);
            timePassed = 0;
        }
    }
}
