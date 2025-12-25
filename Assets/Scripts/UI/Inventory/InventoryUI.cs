using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro.Examples;


public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform slotsParent;
    [SerializeField] InventorySlotUI slotPrefab;
    [SerializeField] GameObject inventoryPanelObj;
    [SerializeField] private PlayerRotation cameraScript;
   

    private List<InventorySlotUI> allocatedSlots = new List<InventorySlotUI>();

    [Header("Input")]
    [SerializeField] private InputActionReference inventoryInput;

    [SerializeField] bool isInventoryOpen = false;

    [SerializeField] private DescriptionUI descriptionPanel;

    private void Start()
    {
        int maxCapacity = InventoryManager.instance.capacity;

        for (int i = 0; i < maxCapacity; i++)
        {
            InventorySlotUI newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.gameObject.SetActive(false);
            allocatedSlots.Add(newSlot);
        }

        InventoryManager.instance.onInventoryChanged += UpdateUI;
        UpdateUI();

        inventoryPanelObj.SetActive(false);
    }
    private void OnEnable()
    {
        inventoryInput.action.Enable();
        inventoryInput.action.performed += ToggleInventory;
    }
    private void OnDisable()
    {
        inventoryInput.action.performed -= ToggleInventory;
        inventoryInput.action.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext ctx) 
    {
        UpdateUI();

        isInventoryOpen = !isInventoryOpen;
        inventoryPanelObj.SetActive(isInventoryOpen);
     

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (cameraScript != null) cameraScript.canLook = false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (cameraScript != null) cameraScript.canLook = true;
        }

    }
    public void OnSlotSelected(InventorySlot slot)
    {
        descriptionPanel.SetDescripton(slot);
    }

    private void OnDestroy()
    {
        if (InventoryManager.instance != null)
        { InventoryManager.instance.onInventoryChanged -= UpdateUI; }
    }

    void UpdateUI() 
    {
        var dataList = InventoryManager.instance.inventorySlots;

        for (int i = 0; i < allocatedSlots.Count; i++)
        {
            if (i < dataList.Count)
            {
                allocatedSlots[i].gameObject.SetActive(true); 
                allocatedSlots[i].SetItem(dataList[i]);       
            }
            else
            {
                allocatedSlots[i].ClearSlots();      
                allocatedSlots[i].gameObject.SetActive(true); 
            }
        }

      
    }

}
