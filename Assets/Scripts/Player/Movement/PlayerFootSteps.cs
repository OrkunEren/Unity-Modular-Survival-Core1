using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] Transform footTransform;

    [Header("Dynamic Step Settings")]
    [SerializeField] float minStepDistance = 0.5f;
    [SerializeField] float maxStepDistance = 1.2f;
    [SerializeField] float maxSpeed = 8f;

    [Header("Anti-Stacking (Cooldown)")]
    [SerializeField] float stepCooldown = 0.25f; 
    private float lastStepTime; 


    float accumulatedDistance; 
    Vector3 lastPosition;



    [Space]
    [SerializeField] AudioClip[] defaultSounds;
    [SerializeField] AudioClip[] grassSounds;
    [SerializeField] AudioClip[] concreteSounds;
    [SerializeField] AudioClip[] snowSounds;
    [SerializeField] AudioClip[] waterSounds;

    [Header("Wet Sounds")]
    [SerializeField] AudioClip[] mudSounds;


    private Player player;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        lastPosition = transform.position;
    }
    private void Update()
    {
        CheckMovement();
    }
    void CheckMovement()
    {

        float currentSpeed = 0f;
        if (characterController != null)
        {

            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
            currentSpeed = horizontalVelocity.magnitude;
        }
        else
        {
            float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                          new Vector3(lastPosition.x, 0, lastPosition.z));
            currentSpeed = dist / Time.deltaTime;
        }

        bool isGrounded = characterController != null ? characterController.isGrounded : true; 
        if (!isGrounded || currentSpeed < 0.1f)
        {
            lastPosition = transform.position;
            accumulatedDistance = 0f; 
            return;
        }

        float speedRatio = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);

        float currentStepDistance = Mathf.Lerp(minStepDistance, maxStepDistance, speedRatio);

        float distanceMoved = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                               new Vector3(lastPosition.x, 0, lastPosition.z));

        accumulatedDistance += distanceMoved;


        if (accumulatedDistance >= currentStepDistance)
        {
         
            if (Time.time - lastStepTime > stepCooldown)
            {
                PlayStepSound();
                lastStepTime = Time.time; 
                accumulatedDistance = 0f;
            }
            else
            {
                accumulatedDistance = currentStepDistance;
            }
        }

        lastPosition = transform.position;
    }

    public void PlayStepSound()
    {
      
        if (Physics.Raycast(footTransform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.0f))
        {
            AudioClip clipToPlay = null;

            bool isSurfaceWet = false;

            if (WeatherController.instance != null && WeatherController.instance.currentIntensity > 0.4f) 
            {
                if (ShelterDetector.instance == null || !ShelterDetector.instance.isUnderRoof)
                {
                    isSurfaceWet = true;
                }
            }
            
                switch (hit.collider.tag) 
                {
                case "Grass":         
                    if (isSurfaceWet && mudSounds.Length > 0)
                    {
                        clipToPlay = mudSounds[Random.Range(0, mudSounds.Length)];
                    }
                    else
                    {
                        clipToPlay = grassSounds[Random.Range(0, grassSounds.Length)];
                    }                        
                    break;
                case "Concrete":
                    clipToPlay = concreteSounds[Random.Range(0, concreteSounds.Length)];                 
                    break;
                case "Water":
                    clipToPlay = waterSounds[Random.Range(0, waterSounds.Length)];
                    break;
                case "Snow":
                    clipToPlay = snowSounds[Random.Range(0, snowSounds.Length)];
                    break;
                default:
                    clipToPlay = defaultSounds[Random.Range(0,defaultSounds.Length)];
                    break;                    
                }

            if (clipToPlay != null) 
            {
                float randomPitch = Random.Range(.85f, 1.15f);
                AudioManager.instance.PlaySFX(clipToPlay, randomPitch);
            }         
        }
    }
}
