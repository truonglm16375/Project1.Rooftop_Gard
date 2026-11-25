using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class Inventory : MonoBehaviour
{
    public List<ItemStack> slots = new List<ItemStack>();
    public static Inventory Instance {get; private set;}

    // =================================================
    [Header("Test Inspector")]
    [SerializeField] private SeedData testSeed; // Ô để kéo TomatoSeed vào
    [ContextMenu("Test: Thêm 5 hạt giống vào kho")]
    public void AddTestSeed()
    {
        if (testSeed == null) return;
        AddItem(testSeed, 5);
        Debug.Log("Đã thêm 5 " + testSeed.itemName); // itemName kế thừa từ ItemData[cite: 10, 11]
    }
    // =================================================


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

    public void AddItem(ItemData itemToAdd, int amount)
    {
        // Nếu trong kho đã có sẵn hạt này thì cộng dồn số lượng
        foreach (ItemStack slot in slots)
        {
            if (slot.data == itemToAdd)
            {
                slot.stock += amount;
                return;
            }
        }

        // Nếu chưa có thì tạo ô mới[cite: 8, 9]
        ItemStack newStack = new ItemStack { data = itemToAdd, stock = amount };
        slots.Add(newStack);
    }
}
