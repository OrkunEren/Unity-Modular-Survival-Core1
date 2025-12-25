using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Settings")]
    public GameObject objectToPlace;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public float maxBuildDistance = 5f;

    [Header("Visuals")]
    public Material validMaterial;   
    public Material invalidMaterial; 

    private GameObject pendingObject;
    private Camera mainCamera;
    private Renderer[] objectRenderers;
    private bool isValidPosition = true;

    public InputActionReference placeAction;
    public InputActionReference rotateAction;

    private void Awake()
    {
        if (instance != null && instance == this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        mainCamera = Camera.main;
    }

    private void Start()
    {      
         SelectObject(objectToPlace);
    }



    private void OnEnable()
    {
        placeAction.action.Enable();
        if (rotateAction != null) rotateAction.action.Enable();
    }

    private void OnDisable()
    {
        placeAction.action.Disable();
        if (rotateAction != null) rotateAction.action.Disable();
    }

    public void SelectObject(GameObject prefab)
    {
        objectToPlace = prefab;

        if (pendingObject != null) Destroy(pendingObject);

        pendingObject = Instantiate(objectToPlace);

        objectRenderers = pendingObject.GetComponentsInChildren<Renderer>();

        foreach (var collider in pendingObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    private void Update()
    {
        if (pendingObject == null) return;

        UpdateObjectPosition();
        CheckValidity();

        if (placeAction.action.WasPerformedThisFrame() && isValidPosition)
        {
            PlaceObject();
        }

        if (rotateAction != null && rotateAction.action.WasPerformedThisFrame())
        {
            pendingObject.transform.Rotate(Vector3.up, 90f);
        }
    }

    void UpdateObjectPosition()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

      
        if (Physics.Raycast(ray, out hit, maxBuildDistance, groundLayer))
        {
            pendingObject.SetActive(true); 
            pendingObject.transform.position = hit.point;
        }
        else
        {
            pendingObject.SetActive(false);
        }
    }

    void CheckValidity()
    {
        if (Physics.CheckSphere(pendingObject.transform.position, 1.5f, obstacleLayer))
        {
            isValidPosition = false;
            UpdateMaterials(invalidMaterial);
        }
        else
        {
            isValidPosition = true;
            UpdateMaterials(validMaterial);
        }
    }

    void UpdateMaterials(Material mat)
    {
        foreach (var rend in objectRenderers)
        {
            rend.material = mat;
        }
    }

    void PlaceObject()
    {
        Instantiate(objectToPlace, pendingObject.transform.position, pendingObject.transform.rotation);

        Destroy(pendingObject);
        pendingObject = null;

    }

    private void OnDrawGizmos()
    {
        if (pendingObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pendingObject.transform.position, 1.5f);
        }
    }
}