using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoundedUIManager : MonoBehaviour
{
    public static RoundedUIManager instance;

    public InputActionReference viewUI;
    public float timeSlowAmount = 0.2f; 
    public float deadZone = 50f; 

    [Header("Images")]
    public GameObject menuContainer; 
    public Transform arrowIndicator; 
    public List<Transform> options; 

    private int currentSelection = -1;
    private float sectorDegree;
    private bool isMenuOpen = false;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }


    private void OnEnable()
    {
        viewUI.action.Enable();
    }

    private void OnDestroy()
    {
        viewUI.action.Disable();
    }

    private void Start()
    {
        sectorDegree = 360f / options.Count;
    }

    private void Update()
    {
        bool isPressed = viewUI.action.IsPressed();

        if (isPressed)
        {
            if (!isMenuOpen) OpenMenu();
            ProcessRadialInput();     
        }
        else
        {
            if (isMenuOpen) CloseMenuAndExecute();
        }
    }

    void OpenMenu() 
    {
        isMenuOpen = true;
        menuContainer.SetActive(true);
        Time.timeScale = timeSlowAmount;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseMenuAndExecute() 
    {
        isMenuOpen = false;
        menuContainer.SetActive(false);
        Time.timeScale = 1f;

        if (currentSelection != -1)
        {
            Debug.Log(options[currentSelection].name);
        }

        currentSelection = -1;
        ResetVisuals();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ProcessRadialInput() 
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 direction = mousePos - screenCenter;

        if (direction.magnitude < deadZone)
        {
            currentSelection = -1;
            ResetVisuals();
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        angle += (sectorDegree / 2);

        angle = angle % 360;

        if (arrowIndicator != null)
        {
            arrowIndicator.rotation = Quaternion.Euler(0, 0, angle);
        }


        currentSelection = (int)(angle / sectorDegree);

    
        if (currentSelection >= options.Count) currentSelection = 0;


        UpdateVisuals();

    }

    void UpdateVisuals()
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (i == currentSelection)
            {
               
                options[i].localScale = Vector3.one * 1.2f;
                options[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                
                options[i].localScale = Vector3.one;
                options[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    void ResetVisuals()
    {
        foreach (var opt in options)
        {
            opt.localScale = Vector3.one;
            opt.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

}

