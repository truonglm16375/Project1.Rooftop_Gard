using UnityEngine;
using System;
using System.Collections;

[Serializable]
public struct PlantStateData {
    public PlantState state;
    public Sprite sprite;
    public int daysNeeded;
}

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Game Data/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public PlantStateData[] stateData;
    public bool canRegrow;
    public int daysUntilWithered; // Number of days without water before the plant withers
    public Sprite seedIcon; // for purchase and seed selection menu

    public Sprite GetSpriteforState (PlantState target)
    {
        foreach (var item in stateData) {
            if (item.state == target) return item.sprite;
        }
        return null;
    }

    public int GetDaysNeeded (PlantState target) {
        foreach (var item in stateData)
        {
            if (item.state == target) return item.daysNeeded;
        }
        return -1;
    }
    /* For future development
    public float waterRequirement; // Amount of water required for the plant to grow
    public float sunlightRequirement; // Amount of sunlight required for the plant to grow
    */
}