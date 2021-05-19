using UnityEngine;

namespace GameCamera
{
    public class CameraRig : MonoBehaviour
    {
        public Camera RigCamera { get; private set; }
        public GameObject Target { get; set; }

        [SerializeField] private Camera rigCamera;
        [SerializeField] private GameObject target;

        [SerializeField] private bool canMoveOnXAxis;
        [SerializeField] private bool canMoveOnYAxis;
        [SerializeField] private bool canMoveOnZAxis;

        [SerializeField] private bool useLookAt = true;

        [SerializeField] private float moveWeight = 0.01f;

        public bool setTargetExternally = false;

        private void Start()
        {
            RigCamera = rigCamera;

            if (!setTargetExternally)
            {
                Target = target;
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
            targetPosition = Target ? Target.gameObject.transform.position : Vector3.zero;

            return targetPosition;
        }

        private void FollowTarget(Vector3 pTargetPosition)
        {
            Vector3 newPosition = transform.position;
            Vector3 targetPosition = pTargetPosition;
            Vector3 rigPosition = transform.position;

            if (canMoveOnXAxis)
            {
                newPosition.x += (targetPosition.x - rigPosition.x) * moveWeight;
            }

            if (canMoveOnYAxis)
            {
                newPosition.y += (targetPosition.y - rigPosition.y) * moveWeight;
            }

            if (canMoveOnZAxis)
            {
                newPosition.z += (targetPosition.z - rigPosition.z) * moveWeight;
            }

            transform.position = newPosition;

            if (useLookAt)
            {
                rigCamera.transform.LookAt(pTargetPosition);
            }
        }
    }
}