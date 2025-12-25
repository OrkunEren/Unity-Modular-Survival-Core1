using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float reachDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform cameraRoot;

    [Header("Input")]
   
    [SerializeField] private InputActionReference interactInput;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI interactText;

   
    private IInteractable currentInteractable;

    private void OnEnable()
    {      
        interactInput.action.Enable();
     
        interactInput.action.performed += OnInteract;
    }

    private void OnDisable()
    {
     
        interactInput.action.performed -= OnInteract;
        interactInput.action.Disable();
    }

    private void Update()
    {
        CheckForInteractable();
    }

  
    void CheckForInteractable()
    {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(cameraRoot.position, cameraRoot.forward, out hit, reachDistance, interactableLayer);


    
            if (hitSomething && hit.collider.TryGetComponent(out IInteractable interactable))
            {
               
                currentInteractable = interactable;

             
                if (!interactText.gameObject.activeSelf || interactText.text != interactable.GetDescriptionText())
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = interactable.GetDescriptionText();
                }
                return;
            }
  

       
        currentInteractable = null;
        if (interactText.gameObject.activeSelf) interactText.gameObject.SetActive(false);
    }

    
    private void OnInteract(InputAction.CallbackContext context)
    {
       
        if (currentInteractable != null  )
        {
            currentInteractable.Interact();
        }
    }
}