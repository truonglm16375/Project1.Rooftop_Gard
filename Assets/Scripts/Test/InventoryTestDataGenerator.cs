
using UnityEngine;
using System.Collections.Generic;

public class InventoryTestDataGenerator : MonoBehaviour
{
    private Inventory inventory => Inventory.Instance;

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        if (inventory == null) return;
        inventory.slots.Clear();
        Debug.Log("[TestData] Inventory cleared.");
    }

    [ContextMenu("Generate Seeds")]
    public void GenerateSeeds()
    {
        if (inventory == null)
        {
            Debug.LogError("[TestData] Inventory instance not found.");
            return;
        }

        ItemStack CreateSeedStack(string plantAssetName, int quantity = 5)
        {
            // Load the ScriptableObject asset at runtime.
            var seedData = Resources.Load<SeedData>(plantAssetName);
            if (seedData == null)
            {
                Debug.LogWarning($"[TestData] PlantData '{plantAssetName}' not found in Resources.");
                return null;
            }
            return new ItemStack { data = seedData, stock = quantity };
        }

        // Example seeds – adjust asset names to match your project.
        var seeds = new List<ItemStack>
        {
            CreateSeedStack("TomatoSeed"),
            CreateSeedStack("Plants/CarrotSeed"),
            CreateSeedStack("Plants/LettuceSeed")
        };

        // Add non‑null stacks to the inventory.
        foreach (var stack in seeds)
        {
            if (stack != null) inventory.slots.Add(stack);
        }

        Debug.Log($"[TestData] Generated {inventory.slots.Count} seed stacks for testing.");
    }
}
