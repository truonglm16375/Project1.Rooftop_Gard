
using UnityEngine;
using System.Collections.Generic;

public class InventoryTestDataGenerator : MonoBehaviour
{
    // Reference to the singleton Inventory instance.
    private Inventory inventory => Inventory.Instance;

    /// <summary>
    /// Clears all items currently stored in the Inventory.
    /// </summary>
    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        if (inventory == null) return;
        inventory.item.Clear();
        Debug.Log("[TestData] Inventory cleared.");
    }

    /// <summary>
    /// Generates a few seed items for common plants.
    /// The PlantData assets must exist in the project under "Assets/ScriptableObjects/Plants".
    /// </summary>
    [ContextMenu("Generate Test Seeds")]
    public void GenerateTestSeeds()
    {
        if (inventory == null)
        {
            Debug.LogError("[TestData] Inventory instance not found.");
            return;
        }

        // Ensure the list is empty before adding test items.
        inventory.item.Clear();

        // Helper to create an ItemStack with a given PlantData asset name.
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
            CreateSeedStack("Plants/TomatoSeed"),
            CreateSeedStack("Plants/CarrotSeed"),
            CreateSeedStack("Plants/LettuceSeed")
        };

        // Add non‑null stacks to the inventory.
        foreach (var stack in seeds)
        {
            if (stack != null) inventory.item.Add(stack);
        }

        Debug.Log($"[TestData] Generated {inventory.item.Count} seed stacks for testing.");
    }
}
