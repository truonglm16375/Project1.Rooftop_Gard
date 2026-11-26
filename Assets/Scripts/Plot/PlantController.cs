using UnityEngine;
using System;

public enum PlantState 
{
    // Cây tái sinh (canRegrow): Seed -> Sprout -> Growing -> Mature -> Harvested -> Mature...
    // Cây một vụ: Seed -> Sprout -> Growing -> Mature -> (Thu hoạch / Hủy)
    // Cây thiếu nước quá số giờ quy định: Withered (Héo úa)
    Seed,       // 0: Hạt giống
    Sprout,     // 1: Mầm non
    Growing1,    // 2: Đang lớn
    Growing2,
    Growing3,   // Cây về trạng thái này sau khi Harvest
    Mature, 
    // Withered - future development
}

public class PlantController : MonoBehaviour 
{
    [Header("Dữ liệu cây trồng")]
    [SerializeField] private PlantData plantData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Trạng thái cây
    public PlantState currentState { get; private set; } = PlantState.Seed;
    public int hoursDelta { get; private set; } = 0;
    public int hoursNeededForNextState { get; private set; } = 0;
    public bool isPlanted => plantData != null;
    public PlantData CurrentPlantData => plantData;

    // Event thông báo khi cây đổi trạng thái
    public static event Action<PlantState> OnPlantStateChanged;
    
    private void Awake() 
    {
        // Đảm bảo hiển thị cây nổi trên bề mặt ô đất
        SpriteRenderer parentRenderer = GetComponentInParent<SpriteRenderer>();
        if (parentRenderer != null && spriteRenderer != null)
        {
            spriteRenderer.sortingLayerID = parentRenderer.sortingLayerID;
            spriteRenderer.sortingOrder = parentRenderer.sortingOrder + 1;
        }
    }

    private void Start() 
    {

    }

    // Xử lí event
    private void OnEnable() 
    {
        GameClock.OnHourPassed += HandleHourPassed;
    }

    private void OnDisable() 
    {
        GameClock.OnHourPassed -= HandleHourPassed;
    }

    // Bắt đầu gieo hạt giống
    public void Plant(PlantData data)
    {
        if (data == null) return;

        plantData = data;
        hoursDelta = 0;
        ChangePlantState(PlantState.Seed);
    }

    public void Harvest()
    {
        if (currentState != PlantState.Mature) return;
        Inventory.Instance.AddItem(plantData.plantProduct, UnityEngine.Random.Range(plantData.minYield, plantData.maxYield));
        if (plantData.canRegrow)
        {
            ChangePlantState(PlantState.Growing3);
        }
        else
        {
            ClearPlant();
        }
    }

    // Xóa cây (khi dọn ô đất hoặc reset)
    public void ClearPlant()
    {
        plantData = null;
        hoursDelta = 0;
        hoursNeededForNextState = 0;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
        gameObject.SetActive(false);
    }
    
    // Xử lí OnHourPassed do GameClock invoke
    private void HandleHourPassed(int hoursPassed)
    {
        if (!isPlanted) return;
        Grow(hoursPassed);
    }

    // Logic tăng trưởng
    public void Grow(int hours = 1)
    {
        if (!isPlanted) return;
        if (currentState == PlantState.Mature) return;

        hoursDelta += hours;

        // Tăng trưởng tuần tự: Seed (0) -> Sprout (1) -> Growing 1,2,3 (2,3,4) -> Mature (5)
        while (hoursNeededForNextState > 0 && hoursDelta >= hoursNeededForNextState)
        {
            hoursDelta -= hoursNeededForNextState;
            if (currentState < PlantState.Mature) ChangePlantState(currentState + 1);
            else {
                hoursDelta = 0;
                break;
            }
        }
    }

    // Chuyển đổi trạng thái cây và cập nhật Sprite
    public void ChangePlantState(PlantState newState)
    {
        currentState = newState;
        OnPlantStateChanged?.Invoke(currentState);

        spriteRenderer.sprite = plantData.GetSpriteforState(currentState);
        hoursNeededForNextState = plantData.GetTimeNeeded(currentState);
    }
}