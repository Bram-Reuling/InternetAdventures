using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class Player : MonoBehaviour
    {
        private Vector3 checkPoint;

        private CharacterController characterController;
        private PlayerInput playerInput;
        private CharacterMovement characterMovement;
        private ObjectInteraction objectInteraction;
        private Ziplinee ziplinee;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            characterMovement = GetComponent<CharacterMovement>();
            objectInteraction = GetComponent<ObjectInteraction>();
            ziplinee = GetComponent<Ziplinee>();
        }

        public void SetCheckPoint(Vector3 pPosition)
        {
            //Debug.Log("Set Check Point to: " + pPosition.ToString());
            checkPoint = pPosition;
        }

        public void RespawnPlayer()
        {
            //Debug.Log("Respawning to: " + checkPoint.ToString());
            
            SetComponentsActive(false);
            gameObject.transform.position = checkPoint;
            SetComponentsActive(true);
        }

        private void SetComponentsActive(bool pBool)
        {
            characterController.enabled = pBool;
            playerInput.enabled = pBool;
            characterMovement.enabled = pBool;
            objectInteraction.enabled = pBool;
            ziplinee.enabled = pBool;
        }
    }
}