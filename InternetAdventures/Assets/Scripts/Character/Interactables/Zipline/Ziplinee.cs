using UnityEngine;
using UnityEngine.InputSystem;

public class Ziplinee : MonoBehaviour
{
    //Public
    [SerializeField] private float minDistanceToZipline;
    [SerializeField] private float ziplineSpeed;
    
    //Private
    private CharacterController _characterController;
    private CharacterMovement _characterMovement;
    private PlayerInput _playerInput;
    private Vector3 _ziplineVector;
    private Zipline _currentZipline;

    private void Start()
    {
        //Get components
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _characterMovement = GetComponent<CharacterMovement>();
        //Setup input
        _playerInput.actions.FindAction("Jump").performed += AttachToZipline;
        _playerInput.actions.FindAction("Jump").canceled += DetachFromZipline;
    }

    private void Update()
    {
        if (_ziplineVector != Vector3.zero)
        {
            _characterController.Move(Time.deltaTime * ziplineSpeed * _ziplineVector);
            if(!_currentZipline.ZiplineMovementValid(transform.position)) DetachFromZipline();
        }
    }

    private void AttachToZipline(InputAction.CallbackContext pCallback)
    {
        /*
        Info: Checks if character is in air, then it's going to fetch all zipline in the scene using tags.
        First check is whether the zipline is usable at all. I then get the shortest vector to the line,
        after that I check whether that value is shorter than my current one and if so replace the zipline to use.
        */
        if (!_characterController.isGrounded)
        {
            Zipline currentBestZipline = null;
            float currentShortestVector = float.PositiveInfinity;
            GameObject[] ziplines = GameObject.FindGameObjectsWithTag("Zipline");
            foreach (var zipline in ziplines)
            {
                Zipline currentZipline = zipline.GetComponent<Zipline>();
                if (!currentZipline.ZiplineUsable(transform.position)) continue;
                Vector3 PlayerToLineVec = currentZipline.GetShortestVectorToLine(transform.position);
                if (PlayerToLineVec.magnitude <= minDistanceToZipline)
                {
                    if (PlayerToLineVec.magnitude < currentShortestVector)
                    {
                        currentBestZipline = currentZipline;
                        currentShortestVector = PlayerToLineVec.magnitude;
                        _ziplineVector = currentZipline.GetNormalizedDirectionVector();
                        _ziplineVector *= Mathf.Sign(Vector3.Dot(transform.forward, _ziplineVector));
                    }

                    _currentZipline = currentBestZipline;
                    _characterMovement.UserInputAllowed = false;
                }
            }
        }
    }

    private void DetachFromZipline(InputAction.CallbackContext pCallback)
    {
        if (_ziplineVector == Vector3.zero) return;
        _ziplineVector = Vector3.zero;
        _characterMovement.UserInputAllowed = true;
        _characterMovement.AddJumpForce();
    }
    
    //Had to overload the method since apparently CallbackContext has no proper default value I could use as standard parameter :(.
    private void DetachFromZipline()
    {
        if (_ziplineVector == Vector3.zero) return;
        _ziplineVector = Vector3.zero;
        _characterMovement.UserInputAllowed = true;
        _characterMovement.AddJumpForce();
    }
}
