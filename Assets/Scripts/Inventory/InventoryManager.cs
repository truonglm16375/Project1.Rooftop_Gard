using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class InventoryManager : MonoBehaviour
{
    public List<ItemStack> slots = new List<ItemStack>();
    private static InventoryManager _instance;
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InventoryManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("InventoryManager");
                    _instance = go.AddComponent<InventoryManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public List<ItemStack> GetItemsByType<T>() where T : ItemData
    {
        List<ItemStack> filteredList = new List<ItemStack>();
        foreach (ItemStack slot in slots)
        {
            if (slot.data != null && slot.data is T)
            {
                filteredList.Add(slot);
            }
        }
        return filteredList;
    }
}
