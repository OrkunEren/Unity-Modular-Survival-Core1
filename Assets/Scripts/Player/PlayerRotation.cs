using UnityEngine;
using Unity.Cinemachine; 

public class PlayerRotation : MonoBehaviour
{
    [Header("References")]
    public Player player;
    
    [SerializeField] CinemachineCamera fpsCamera;
    [SerializeField] Transform cameraRoot; 

    [Header("Settings")]
    [SerializeField] float sensitivityX = 20f;
    [SerializeField] float sensitivityY = 20f;
    [SerializeField] float upperLimit = -80f;
    [SerializeField] float lowerLimit = 80f;

    private float _xRotation;
    private Vector2 _currentInputVector;
    private Vector2 _smoothInputVelocity;
    [SerializeField] float smoothTime = 0.05f;

    public bool canLook = true;


    private void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !canLook;
    }

    private void Update()
    {
        if (!canLook)
            return;

        HandleRotation();
    }

    void HandleRotation()
    {
 
        Vector2 rawInput = player.inputs.Player.Look.ReadValue<Vector2>();
        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, rawInput, ref _smoothInputVelocity, smoothTime);

        float mouseX = _currentInputVector.x * sensitivityX * Time.deltaTime;
        float mouseY = _currentInputVector.y * sensitivityY * Time.deltaTime;

        
        transform.Rotate(Vector3.up * mouseX);

        
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, upperLimit, lowerLimit);

        if (cameraRoot != null)
        {
            cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
    } 
    
}