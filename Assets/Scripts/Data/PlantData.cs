using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public struct PlantStateData 
{
    public PlantState state;
    public Sprite sprite;

    [FormerlySerializedAs("daysNeeded")]
    [Tooltip("Số giờ cần để phát triển sang giai đoạn tiếp theo")]
    public int timeNeeded; // Tính theo giờ (hours)
}

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Game Data/Plant Data")]
public class PlantData : ScriptableObject
{
    [FormerlySerializedAs("plantName")]
    [Header("Thông tin chung")]
    public string Name;

    [FormerlySerializedAs("seedIcon")]
    public Sprite icon;

    [Header("Giai đoạn sinh trưởng")]
    public PlantStateData[] stateData;

    [Header("Đặc tính cây")]
    [Tooltip("Cây có thể tái sinh trưởng sau khi thu hoạch")]
    public bool canRegrow;

    [FormerlySerializedAs("daysUntilWithered")]
    [Tooltip("Số giờ không được tưới nước trước khi cây bị héo úa")]
    public int timeUntilWithered; // Tính theo giờ (hours)

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