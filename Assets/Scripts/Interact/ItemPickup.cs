using UnityEngine;

public class ItemPickup : MonoBehaviour , IInteractable
{
    [SerializeField] ItemData itemData;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public string GetDescriptionText() 
    {
        return "E - " + itemData.itemName + " Collect"; 
    }

    public void Interact() 
    {
        bool isAdded = InventoryManager.instance.AddItem(itemData);

        if (isAdded)
        {
            Destroy(gameObject);
        }
 
    }
}
