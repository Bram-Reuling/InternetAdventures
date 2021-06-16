using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

        public bool setTargetExternally = false;

        private Vector3 cameraStartPosition;
        private Vector3 cameraDestinationPosition;

        private Quaternion cameraStartRotation;
        private Vector3 cameraDestinationRotation;

        private float speed = 0;
        private float fraction = 0;

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

            if (!setTargetExternally)
            {
                Target = target;    
            }

            var transform1 = RigCamera.transform;
            var position = transform1.localPosition;
            var rotation = transform1.localRotation;
            cameraStartPosition = position;
            cameraStartRotation = rotation;

            cameraDestinationPosition = position;
            cameraDestinationRotation = rotation.eulerAngles;
            
            transform.position = DetermineTargetPosition(new Vector3());
        }

        private void Update()
        {
            Vector3 targetPosition = new Vector3();

            targetPosition = DetermineTargetPosition(targetPosition);

            FollowTarget(targetPosition);

            if (fraction < 1)
            {
                fraction += Time.deltaTime * speed;
                RigCamera.transform.localPosition =
                    Vector3.Lerp(cameraStartPosition, cameraDestinationPosition, fraction);
                RigCamera.transform.localRotation = Quaternion.Lerp(cameraStartRotation, Quaternion.Euler(cameraDestinationRotation), fraction);
            }
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
                camera.transform.LookAt(pTargetPosition);   
            }
        }

        public void ChangePerspective(Vector3 pOffset, Vector3 pRotOffset, float pTime)
        {
            speed = pTime;
            fraction = 0;
            
            var rigCameraTransform = RigCamera.transform;
            var position = rigCameraTransform.localPosition;
            cameraStartPosition = position;
            cameraStartRotation = rigCameraTransform.localRotation;

            cameraDestinationPosition = position + pOffset;
            cameraDestinationRotation = cameraStartRotation.eulerAngles + pRotOffset;
        }
    }
}
