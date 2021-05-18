using UnityEngine.InputSystem;

public class Idle : CharacterState
{
    public override void Initialize(CharacterFSM pCharacterFSM)
    {
        base.Initialize(pCharacterFSM);
        PlayerInput characterInput = pCharacterFSM.CharacterFsmInformation.PlayerInput;
        characterInput.actions.FindAction("Jump").performed += OnJump;
        characterInput.actions.FindAction("Movement").performed += OnMovement;
    }

    private void OnJump(InputAction.CallbackContext pCallback)
    {
        _characterFSM.ChangeState<Jump>();
    }
    
    private void OnMovement(InputAction.CallbackContext pCallback)
    {
        _characterFSM.ChangeState<Walk>();
    }
}
