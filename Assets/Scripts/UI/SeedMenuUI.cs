using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SeedMenuUI : BaseUIPanel
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    void OnEnable()
    {

    }
    protected override void OnShow()
    {
        // Xoa button cu va Instantiate button moi
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);
        List<ItemStack> seedStacks = Inventory.Instance.GetItemsByType<SeedData>();
        foreach (ItemStack seed in seedStacks)
        {
            GameObject newBtn = Instantiate(buttonPrefab, buttonContainer);
            if (seed.data is SeedData seedData)
                newBtn.GetComponent<SeedButtonUI>().SetUp(seedData.plantData);
        }
    }

    protected override void OnHide()
    {
        
    }


}