using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour
{
    [Header("Description Panel")]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image descriptionImage;
    [SerializeField] private TextMeshProUGUI itemStats;

    [SerializeField] private GameObject parentObject;
    private void Awake()
    {
        ResetDescription();
    }

    public void SetDescripton(InventorySlot slot)
    {
        if (slot == null || slot.item == null) return;

        itemName.text = slot.item.itemName; 
        description.text = slot.item.description;
        itemStats.text = slot.item.itemType.ToString();

        descriptionImage.sprite = slot.item.icon;
        descriptionImage.gameObject.SetActive(true); 

        if (parentObject != null) parentObject.SetActive(true);
        else gameObject.SetActive(true);
    }

    public void ResetDescription()
    {
        itemName.text = "";
        description.text = "";
        itemStats.text = "";
        descriptionImage.gameObject.SetActive(false);

     
        if (parentObject != null) parentObject.SetActive(false);
        else gameObject.SetActive(false);
    }

}
