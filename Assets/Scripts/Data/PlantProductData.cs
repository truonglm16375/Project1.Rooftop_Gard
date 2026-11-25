using UnityEngine;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPlantProductData", menuName = "Game Data/Plant Product Data")]
public class PlantProductData : ItemData
{
    [Header("Thông tin chung")]
    public string Name;
    public Sprite icon;

 
}