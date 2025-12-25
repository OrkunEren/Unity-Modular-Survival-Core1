using UnityEngine;

public enum ItemType 
{
    None,
    Food,
    Drink,
    Weapon,
    Material
}




public enum ConsumableType
{
    None,
    Health, 
    Hunger, 
    Thirst,
    Temperature
}

[CreateAssetMenu(fileName = "New Item", menuName = "Survival/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Fundamentals")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Header("Consume Info")]
    public ConsumableType consumableType;
    public float value;

    [Header("Specifics")]
    public bool isStackable;
    public int maxStackSize;
    public float weight;

    [Header("Prefab")]
    public GameObject prefab;

    [Space]
    [TextArea] public string description;
}
