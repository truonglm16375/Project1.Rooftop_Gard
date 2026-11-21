using UnityEngine;

[CreateAssetMenu(fileName = "NewSeedData", menuName = "Game Data/Seed Data")]
public class SeedData : ItemData
{
    public int price;
    public int stock;
    public PlantData plantData;
}