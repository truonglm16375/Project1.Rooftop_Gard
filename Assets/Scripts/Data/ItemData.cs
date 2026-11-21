using UnityEngine;

public enum ItemType
{
    Plant,
    Seed
}

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    public ItemType itemType;
}