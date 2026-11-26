using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public struct PlantStateData 
{
    public PlantState state;
    public Sprite sprite;
    public int timeNeeded; // Tính theo giờ (hours)
}

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Game Data/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Thông tin chung")]
    public string Name;
    public Sprite icon;

    [Header("Giai đoạn sinh trưởng")]
    public PlantStateData[] stateData;

    [Header("Đặc tính cây")]
    public bool canRegrow;
    public int minYield;
    public int maxYield;
    public PlantProductData plantProduct;

    public Sprite GetSpriteforState(PlantState target)
    {
        if (stateData == null) return null;
        foreach (var item in stateData) 
        {
            if (item.state == target) return item.sprite;
        }
        return null;
    }

    public int GetTimeNeeded(PlantState target) 
    {
        if (stateData == null) return -1;
        foreach (var item in stateData)
        {
            if (item.state == target) return item.timeNeeded;
        }
        return -1;
    }
    
    /* For future development
    public float waterRequirement; // Amount of water required for the plant to grow
    public float sunlightRequirement; // Amount of sunlight required for the plant to grow
    */
}