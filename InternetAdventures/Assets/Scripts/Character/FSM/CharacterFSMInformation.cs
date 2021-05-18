using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterFSMInformation : MonoBehaviour
{
    //This class holds necessary information that each state can access.
    
    public CharacterController CharacterController { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    //Velocity information
    private Vector3 _velocity;
    public Vector3 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
    }
}
