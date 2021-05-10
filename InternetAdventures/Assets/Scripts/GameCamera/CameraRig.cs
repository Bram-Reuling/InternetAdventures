using UnityEngine;

namespace GameCamera
{
    public class CameraRig : MonoBehaviour
    {
        public Camera RigCamera { get; private set; }
        public GameObject Target { get; set; }
        
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject target;
        
        [SerializeField] private bool canMoveOnXAxis;
        [SerializeField] private bool canMoveOnYAxis;
        [SerializeField] private bool canMoveOnZAxis;

        [SerializeField] private bool useLookAt = true;
        
        [SerializeField] private float moveWeight = 0.01f;

        private void Start()
        {
            if (camera == null)
            {
                Debug.LogError("NO CAMERA HAS BEEN FOUND");
            }
            else
            {
                RigCamera = camera;
            }

            Target = target;
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
                //newPosition.x = FollowAxisPosition(targetPosition.x, rigPosition.x);
                newPosition.x += (targetPosition.x - rigPosition.x) * moveWeight;
            }

            if (canMoveOnYAxis)
            {
                //newPosition.y = FollowAxisPosition(targetPosition.y, rigPosition.y);
                newPosition.y += (targetPosition.y - rigPosition.y) * moveWeight;
            }

            if (canMoveOnZAxis)
            {
                //newPosition.z = FollowAxisPosition(targetPosition.z, rigPosition.z);    
                newPosition.z += (targetPosition.z - rigPosition.z) * moveWeight;    
            }

            //transform.position = Vector3.Lerp(rigPosition, newPosition, 5);
            transform.position = newPosition;

            if (useLookAt)
            {
                camera.transform.LookAt(pTargetPosition);   
            }
        }

        private float FollowAxisPosition(float pTargetAxisValue, float pRigAxisValue)
        {
            pRigAxisValue += (pTargetAxisValue - pRigAxisValue) * 0.1f;
            return pRigAxisValue;
        }
    }
}
