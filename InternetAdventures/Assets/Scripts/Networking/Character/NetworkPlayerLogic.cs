using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking.Character
{
    public class NetworkPlayerLogic : NetworkBehaviour
    {
        #region Variables

        private Vector3 checkPoint;

        private CharacterController characterController;
        private PlayerInput playerInput;
        private NetworkCharacterMovement characterMovement;
        private ObjectInteraction objectInteraction;
        private Ziplinee zipline;

        #endregion

        #region Global Functions

        #endregion

        #region Client Functions

        

        #endregion

        #region Server Functions

        [ServerCallback]
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            characterMovement = GetComponent<NetworkCharacterMovement>();
            objectInteraction = GetComponent<ObjectInteraction>();
            zipline = GetComponent<Ziplinee>();
        }
        
        [ServerCallback]
        public void SetCheckPoint(Vector3 pPosition)
        {
            //Debug.Log("Set Check Point to: " + pPosition.ToString());
            checkPoint = pPosition;
        }
        
        [ServerCallback]
        public void RespawnPlayer()
        {
            //Debug.Log("Respawning to: " + checkPoint.ToString());
            
            SetComponentsActive(false);
            gameObject.transform.position = checkPoint;
            SetComponentsActive(true);
        }
        
        [ServerCallback]
        private void SetComponentsActive(bool pBool)
        {
            characterController.enabled = pBool;
            playerInput.enabled = pBool;
            characterMovement.enabled = pBool;
            objectInteraction.enabled = pBool;
            zipline.enabled = pBool;
        }
        
        #endregion
        
    }
}