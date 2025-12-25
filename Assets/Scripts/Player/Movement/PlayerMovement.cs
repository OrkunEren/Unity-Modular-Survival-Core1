using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Settings")]
    public float walkSpeed = 5f;
    public float backwardSpeed = 3f;
    public float runSpeed = 8f;
    public float strafeSpeed = 2.0f;
    public float runTransitionSmooth = 5f;

    [Header("Gravity")]
    [SerializeField] float gravityValue = -9.81f;
    
    private Player player;
    private CharacterController _controller;

    Vector2 input;


    private float _currentSpeed;
    private float _verticalVelocity;
    private Vector2 _currentInputVector; 
    private Vector2 _smoothInputVelocity; 
    [SerializeField] float moveSmoothTime = 0.1f; 

    private void Awake()
    {
        player = GetComponent<Player>();
        _controller = GetComponent<CharacterController>();
        
    }

    private void Start()
    {
        PlayerInputs();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {     
        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, input, ref _smoothInputVelocity, moveSmoothTime);

       
        
        Vector3 moveDir = ((transform.right * _currentInputVector.x) + (transform.forward * _currentInputVector.y)).normalized;

        float targetSpeed = 0f;

     
        if (input == Vector2.zero)
        {
            targetSpeed = 0f;
        }
        else if (input.y < -0.1f)
        {
            targetSpeed = backwardSpeed; 
        }
        else if (player.inputs.Player.Sprint.IsPressed() && input.y > 0)
        {
            targetSpeed = runSpeed;
        }
        else if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f)
        {
            targetSpeed = strafeSpeed; 
        }
        else
        {
            targetSpeed = walkSpeed; 
        }


        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * runTransitionSmooth);

        
        ApplyGravity();

        
        Vector3 finalMove = (moveDir * _currentSpeed) + (Vector3.up * _verticalVelocity);

        _controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
     
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; 
        }
        
        _verticalVelocity += gravityValue * Time.deltaTime;

        float normalizedAirY = Mathf.Clamp(_verticalVelocity / 5f, -1f, 1f);

    }

    void PlayerInputs() 
    {
        player.inputs.Player.Move.performed += ctx => input = ctx.ReadValue<Vector2>();
        player.inputs.Player.Move.canceled += ctx => input = Vector2.zero;
    }
}