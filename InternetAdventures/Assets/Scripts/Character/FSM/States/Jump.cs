using UnityEngine;

public class Jump : CharacterState
{
    [SerializeField] private float jumpHeight;
    private CharacterFSMInformation _characterFsmInformation;
    private Vector3 _velocity;

    public override void EnterState()
    {
        base.EnterState();
        _characterFsmInformation = _characterFSM.CharacterFsmInformation;
        _velocity = _characterFsmInformation.Velocity;
        AddJumpForce();
    }

    public override void Update()
    {
        base.Update();
        _characterFsmInformation.CharacterController.Move(_velocity * Time.deltaTime);
        
        //Exit condition
        if (_characterFsmInformation.CharacterController.isGrounded)
        {
            if(_velocity.magnitude < 0.01f) _characterFSM.ChangeState<Idle>();
            else _characterFSM.ChangeState<Walk>();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        _characterFsmInformation.Velocity = _velocity;
    }

    private void AddJumpForce()
    {
        if (_characterFSM.CharacterFsmInformation.CharacterController.isGrounded)
             _velocity.y = jumpHeight;
    }
}
