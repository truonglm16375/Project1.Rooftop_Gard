using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotController : MonoBehaviour, IPointerClickHandler
{
[Header("UI Reference")]
    [SerializeField] private GameObject actionMenuCanvas; 
    [SerializeField] private GameObject actionMenu; 

[Header("Plant Settings")]
    [SerializeField] public GameObject plantSpawnPoint; // Object cha cho Plant
    [SerializeField] private PlantController plantController;

    // Trạng thái ô đất và cây trồng
    public bool isPlanted => plantController != null && plantController.isPlanted;
    public PlantData plantedPlant => plantController != null ? plantController.CurrentPlantData : null;
    public PlantState currentState => plantController != null ? plantController.currentState : PlantState.Seed;

    private void Awake()
    {
        InitPlantController();
    }

    private void Start() 
    {
        if (actionMenuCanvas != null) actionMenuCanvas.SetActive(false);
    }

    private void InitPlantController()
    {
        if (plantController == null)
        {
            if (plantSpawnPoint != null)
            {
                plantController = plantSpawnPoint.GetComponent<PlantController>();
                if (plantController == null)
                {
                    plantController = plantSpawnPoint.AddComponent<PlantController>();
                }
            }
            else
            {
                plantController = GetComponentInChildren<PlantController>();
            }
        }
    }

    // ========= Hiển thị / Đóng Menu tương tác ==========
    public void OnPointerClick(PointerEventData eventData) 
    {
        // Ghi nhận ô đất này là ô đất đang được người chơi chọn
        if (PlotManager.Instance != null) PlotManager.Instance.SelectPlot(this);
        if (actionMenuCanvas != null && !actionMenuCanvas.activeSelf) 
            actionMenuCanvas.SetActive(true);
    }

    public void DisableActionMenu()
    {
        if (actionMenuCanvas != null) 
            actionMenuCanvas.SetActive(false);
    }

    // ======= Xử lý gieo hạt giống vào ô đất =========
    public void GieoHat(PlantData data)
    {
        if (isPlanted)
        {
            Debug.LogWarning("Ô đất này đã có cây trồng, không thể gieo thêm!");
            DisableActionMenu();
            return;
        }

        InitPlantController();

        if (plantController != null)
        {
            plantController.Plant(data);
            Debug.Log($"Đã gieo hạt '{data.Name}' vào ô đất!");
        }
        else
        {
            Debug.LogError("Không tìm thấy PlantController trên ô đất để gieo hạt!");
        }

        DisableActionMenu();
    }
}
