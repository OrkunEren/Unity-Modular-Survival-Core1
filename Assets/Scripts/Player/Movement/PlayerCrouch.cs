using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PlayerCrouch : MonoBehaviour
{
    Player player;

    [Header("Crouch settings")]
    [SerializeField] float crouchHeight;
    [SerializeField] float defaultHeight;
    [SerializeField] float crouchLerp;
    float currentHeight;
    bool isCrouch = false;

   

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        PlayerInputs();
        currentHeight = player.characterController.height;
        
    }

    private void Update()
    {
        ApplyCrouch();

    }

    void ApplyCrouch() 
    {
        if (isCrouch)
        {

            player.characterController.height = Mathf.Lerp(currentHeight , crouchHeight , crouchLerp * Time.deltaTime);
            currentHeight = player.characterController.height;

            Vector3 newCenter = player.characterController.center;
            newCenter.y = crouchHeight / crouchHeight;
            player.characterController.center = newCenter;

        }
        else 
        {          
            player.characterController.height = Mathf.Lerp(currentHeight, defaultHeight, crouchLerp * Time.deltaTime); ;
            currentHeight = player.characterController.height;

            Vector3 newCenter = player.characterController.center;
            newCenter.y = defaultHeight / defaultHeight;
            player.characterController.center = newCenter;

        }
    }

     

    void PlayerInputs() 
    {
        PlayerInputs inputs = player.inputs;
        inputs.Player.Crouch.performed += ctx => isCrouch = !isCrouch;
    } 
}
