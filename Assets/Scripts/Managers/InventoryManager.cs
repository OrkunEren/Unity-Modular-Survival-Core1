using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }

    public event Action onInventoryChanged;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    [SerializeField] public int capacity = 10;
    [SerializeField] public float maxWeightToCarry = 2;
    public float currentWeight;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool CanCarry(float itemWeight)
    {
        return (currentWeight + itemWeight) <= maxWeightToCarry;
    }

    public bool AddItem(ItemData item)
    {

        if (!CanCarry(item.weight))
        {
            return false;
        }



        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.item == item && slot.count < item.maxStackSize)
                {

                    slot.count++;
                    currentWeight += item.weight;

                    if (onInventoryChanged != null) onInventoryChanged.Invoke();

                    return true;

                }
            }

        }

        if (inventorySlots.Count < capacity)
        {

            InventorySlot newSlot = new InventorySlot(item, 1);

            inventorySlots.Add(newSlot);


            currentWeight += item.weight;
            if (onInventoryChanged != null) onInventoryChanged.Invoke();

            return true;
        }
        else
        {
            Debug.Log("Çantada yer (Slot) yok!");
            return false;
        }
    }

    public void DropItem(InventorySlot slot)
    {

        if (slot.item.prefab != null)
        {
            Transform playerCamera = Camera.main.transform;
            Vector3 dropPosition = playerCamera.position + (playerCamera.forward * 1.5f);

            GameObject droppedObj = Instantiate(slot.item.prefab, dropPosition, Quaternion.identity);

            Rigidbody rb =  droppedObj.GetComponent<Rigidbody>();

            Vector3 force = (playerCamera.forward );

            rb.AddForce(force, ForceMode.Impulse);

            droppedObj.layer = LayerMask.NameToLayer("Interactable");
        }

        currentWeight -= slot.item.weight;

        slot.count--;

        if (slot.count <= 0)
        {
            inventorySlots.Remove(slot);
        }

        if (onInventoryChanged != null) onInventoryChanged.Invoke();
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.item.itemType == ItemType.Food) 
        {
            StatsManager.instance.ChangeStat(slot.item.consumableType, slot.item.value);

            currentWeight -= slot.item.weight;
            slot.count--;

            if (slot.count <= 0)
            {
                inventorySlots.Remove(slot);
            }

            if (onInventoryChanged != null) onInventoryChanged.Invoke();


        }
    }

}




[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;
    public float weight;

    public InventorySlot(ItemData _item, int _count)
    {

        item = _item;
        count = _count;

    }
}


