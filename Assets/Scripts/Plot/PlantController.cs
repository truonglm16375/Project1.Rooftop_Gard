using UnityEngine;
using System;

public enum PlantState 
{
    // Cây tái sinh (canRegrow): Seed -> Sprout -> Growing -> Mature -> Harvested -> Growing -> Mature...
    // Cây một vụ: Seed -> Sprout -> Growing -> Mature -> (Thu hoạch / Hủy)
    // Cây thiếu nước quá số giờ quy định: Withered (Héo úa)
    Seed,       // 0: Hạt giống
    Sprout,     // 1: Mầm non
    Growing,    // 2: Đang lớn
    Mature,     // 3: Trưởng thành (chín / có thể thu hoạch)
    Harvested,  // 4: Đã thu hoạch (chờ tái sinh quả)
    Withered    // 5: Héo úa
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
    
    // Khởi tạo, thiết lập SpriteRenderer 
    private void Awake() 
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

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
        if (plantData != null)
        {
            Plant(plantData);
        }
        else if (spriteRenderer != null && !isPlanted)
        {
            spriteRenderer.sprite = null;
        }
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
        Inventory.Instance.AddItem(plantData.plantProduct, plantData.yield);
        if (plantData.canRegrow)
        {
            ChangePlantState(PlantState.Harvested);
        }
        else
        {
            ClearPlant();
        }
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
        // Cây đã chín (Mature) hoặc đã héo (Withered) thì không tiếp tục tăng trưởng tự động
        if (currentState == PlantState.Withered || currentState == PlantState.Mature) return;

        hoursDelta += hours;

        // Trường hợp cây sau khi thu hoạch đang trong chu kỳ tái sinh quả (canRegrow)
        if (currentState == PlantState.Harvested && plantData.canRegrow)
        {
            ChangePlantState(PlantState.Growing);
        }

        // Tăng trưởng tuần tự: Seed (0) -> Sprout (1) -> Growing (2) -> Mature (3)
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
}