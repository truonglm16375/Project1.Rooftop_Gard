using UnityEngine;
using System;

public enum PlantState {
    // regrowable plant: Seed -> Sprout -> Growing -> Mature -> Harvested -> Mature
    //                   Can only be actively destroyed by player
    // non-regrowable plant: Seed -> Sprout -> Growing -> Mature -> Destroy
    // plant will be Withered if not be Watered for a set number of days
    Seed,
    Sprout,
    Growing,
    Mature,
    Harvested,
    Withered
}
public class PlantController : MonoBehaviour {
    [SerializeField] private PlantData plantData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public PlantState currentState { get; private set; }
    public int daysDelta;
    public int daysNeededforNextState;

    void Awake() {
        currentState = PlantState.Seed;
        daysDelta = 0;
        daysNeededforNextState = plantData.GetDaysNeeded(currentState);
    }

    void Start() {
        // Initialize the plant state or any other necessary setup
    }

    void onEnable() {
        GameClock.OnDayPassed += Grow;
        // subcribe onclick event of PlantButton and HarvestButtton
    }

    public static event Action<PlantState> OnPlantStateChanged;
    public void ChangePlantState(PlantState newState)
    {
        currentState = newState;
        OnPlantStateChanged?.Invoke(currentState);
        Sprite newSprite = plantData.GetSpriteforState(currentState);
        spriteRenderer.sprite = newSprite;
        daysNeededforNextState = plantData.GetDaysNeeded(currentState);
    }
    
    private void Grow()
    {
        daysDelta++;
        // Mature plant won't grow and won't wither
        // Harvest
        if (currentState == PlantState.Withered || currentState == PlantState.Mature) return;
        if (currentState == PlantState.Harvested && plantData.canRegrow)
        {
            if (daysDelta >= daysNeededforNextState) {
                ChangePlantState(PlantState.Growing);
                daysDelta = 0;
            }
        }
        if (daysDelta >= daysNeededforNextState)
        {
            ChangePlantState(currentState+1);
            daysDelta = 0;
        }
    }

    private void Harvest()
    {
        // canRegrow plant will change to Harvested
        // non-regrow plant will be destroyed
        if (currentState == PlantState.Mature)
        {
            if (plantData.canRegrow)
            {
                ChangePlantState(PlantState.Harvested);
                daysDelta = 0;
            }
            else // destroy logic
            {
                
            }
        } 
    }
}