using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Item Sprite")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Buttons")]
    [SerializeField] private Button slotButton; 
    [SerializeField] private Button dropButton;
    [SerializeField] private Button useButton;

    private InventorySlot currentSlot;
    private InventoryUI mainUI;

    private void Awake() 
    {
       
        mainUI = GetComponentInParent<InventoryUI>();
    }

    public void ClearSlots()
    {
        currentSlot = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";
        nameText.text = "";

       
        if (slotButton) slotButton.interactable = false;
        if (useButton) useButton.interactable = false;
        if (dropButton) dropButton.interactable = false;
    }

    public void SetItem(InventorySlot slot)
    {
        currentSlot = slot;

        iconImage.sprite = slot.item.icon;
        iconImage.enabled = true;
        nameText.text = slot.item.itemName;

        if (slot.count > 1) amountText.text = slot.count.ToString();
        else amountText.text = "";

       

        if (slotButton != null)
        {
            slotButton.interactable = true;
            slotButton.onClick.RemoveAllListeners(); 
            slotButton.onClick.AddListener(OnSlotClicked); 
        }

        if (useButton != null)
        {
            useButton.interactable = true;
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseClicked);
        }

        if (dropButton != null)
        {
            dropButton.interactable = true;
            dropButton.onClick.RemoveAllListeners();
            dropButton.onClick.AddListener(OnDropButtonClicked);
        }
    }

    void OnSlotClicked()
    {
        if (mainUI != null && currentSlot != null)
            mainUI.OnSlotSelected(currentSlot);
    }

    void OnDropButtonClicked()
    {
        if (currentSlot != null)
            InventoryManager.instance.DropItem(currentSlot);
    }

    void OnUseClicked()
    {
        if (currentSlot != null)
        {
            if (mainUI != null) mainUI.OnSlotSelected(currentSlot);
            InventoryManager.instance.UseItem(currentSlot);
        }
    }
}