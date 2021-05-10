using System;
using Character;
using GameCamera;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject cameraRigPrefab;

    public GameObject playerOne;
    public GameObject playerTwo;

    private CharacterMovement playerOneMovement;
    private CharacterMovement playerTwoMovement;

    public GameObject cameraRig;
    private CameraRig cameraRigComponent;
    
    private PlayerInput playerInput;
    
    public bool spawnPlayers = false;

    private ECurrentPlayer currentPlayer;

    void Start()
    {
        if (spawnPlayers)
        {
            SpawnPlayers();   
        }

        Initialize();
    }

    private void SpawnPlayers()
    {
        playerOne = PlayerInput
            .Instantiate(characterPrefab, controlScheme: "CharacterWASD", pairWithDevice: Keyboard.current).gameObject;
        cameraRig = Instantiate(cameraRigPrefab);
        cameraRig.GetComponent<CameraRig>().setTargetExternally = true;
        cameraRig.GetComponent<CameraRig>().Target = playerOne;

        playerTwo = PlayerInput
            .Instantiate(characterPrefab, controlScheme: "CharacterWASD", pairWithDevice: Keyboard.current).gameObject;
        playerTwo.transform.position += new Vector3(2, 0, 0);
    }

    private void Initialize()
    {
        playerOneMovement = playerOne.GetComponent<CharacterMovement>();
        playerTwoMovement = playerTwo.GetComponent<CharacterMovement>();

        cameraRigComponent = cameraRig.GetComponent<CameraRig>();
        
        playerInput = GetComponent<PlayerInput>();

        playerOneMovement.enabled = true;
        playerTwoMovement.enabled = false;

        cameraRigComponent.setTargetExternally = true;
        cameraRigComponent.Target = playerOne;
        
        currentPlayer = ECurrentPlayer.PlayerOne;

        playerInput.actions.FindAction("Switch").performed += SwitchPlayers;
    }

    private void SwitchPlayers(InputAction.CallbackContext obj)
    {
        ECurrentPlayer newPlayer;
        
        switch (currentPlayer)
        {
            case ECurrentPlayer.PlayerOne:
                newPlayer = ECurrentPlayer.PlayerTwo;
                cameraRigComponent.Target = playerTwo;
                playerOneMovement.enabled = false;
                playerTwoMovement.enabled = true;
                break;
            case ECurrentPlayer.PlayerTwo:
                newPlayer = ECurrentPlayer.PlayerOne;
                cameraRigComponent.Target = playerOne;
                playerOneMovement.enabled = true;
                playerTwoMovement.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentPlayer = newPlayer;
    }
}
