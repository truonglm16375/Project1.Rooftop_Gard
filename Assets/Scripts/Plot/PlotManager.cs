using UnityEngine;

public class PlotManager : MonoBehaviour
{
    private static PlotManager _instance;
    public static PlotManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlotManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlotManager");
                    _instance = go.AddComponent<PlotManager>();
                }
            }
            return _instance;
        }
    }

    private PlotController selectedPlot;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    // Được gọi khi người chơi click vào 1 ô đất trên bản đồ
    public void SelectPlot(PlotController plot)
    {
        selectedPlot = plot;
    }

    // Được SeedButtonUI gọi trực tiếp sau khi người chơi chọn loại hạt giống
    public void GieoHat(PlantData plantData)
    {
        if (selectedPlot == null)
        {
            Debug.LogWarning("Chưa có ô đất nào được chọn để gieo hạt!");
            return;
        }
        selectedPlot.GieoHat(plantData);
        selectedPlot = null; 
    }
}
