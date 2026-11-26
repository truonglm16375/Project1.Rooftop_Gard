using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotController : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Reference")]
    [SerializeField] private GameObject actionMenu; 
  

    [Header("Plant Settings")]
    [SerializeField] public GameObject plant; // Object đại diện cho Plant
    [SerializeField] private PlantController plantController;

    // Trạng thái ô đất và cây trồng
    public bool isPlanted => plantController != null && plantController.isPlanted;
    public PlantData plantedPlant => plantController != null ? plantController.CurrentPlantData : null;
    public PlantState currentState => plantController != null ? plantController.currentState : PlantState.Seed;

    private void Awake()
    {

    }

    private void Start() 
    {
        if (actionMenu != null) actionMenu.SetActive(false);
        if (plant != null) plantController = plant.GetComponent<PlantController>();
    }

    // ========= Hiển thị / Đóng Menu tương tác ==========
    public void OnPointerClick(PointerEventData eventData) 
    {
        // Ghi nhận ô đất này là ô đất đang được người chơi chọn
        if (PlotManager.Instance != null) PlotManager.Instance.SelectPlot(this);
        if (actionMenu != null && !actionMenu.activeSelf) 
            actionMenu.SetActive(true);
    }

    public void DisableActionMenu()
    {
        if (actionMenu != null) 
            actionMenu.SetActive(false);
    }

    // ======= Xử lý gieo hạt giống vào ô đất =========
    public void GieoHat(PlantData data)
    {
        if (!plant.activeSelf) plant.SetActive(true);

        plantController.Plant(data);
        Debug.Log($"Đã gieo hạt '{data.Name}' vào ô đất!");
        DisableActionMenu();
    }

    public void ThuHoach()
    {
        plantController.Harvest();
        DisableActionMenu();
    }
}
