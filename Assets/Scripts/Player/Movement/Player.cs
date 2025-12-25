using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerMovement movement { get; private set;}
    public PlayerRotation rotation { get; private set; }
    public CharacterController characterController { get; private set; }
    public PlayerCrouch slide { get; private set; }
 
    public PlayerAttacker attacker { get; private set; }
  
    public PlayerInputs inputs { get; private set; }
    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rotation = GetComponent<PlayerRotation>();
        characterController = GetComponent<CharacterController>();
        slide = GetComponent<PlayerCrouch>();
        attacker = GetComponent<PlayerAttacker>();
        inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }
}
