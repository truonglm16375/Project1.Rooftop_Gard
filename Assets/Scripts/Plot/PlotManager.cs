using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public static PlotManager Instance {get ; private set;}
    private PlotController selectedPlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectPlot(PlotController plot)
    {
        selectedPlot = plot;
    }

    // GieoHat cần qua trung gian SeedMenu nên yêu cầu PlotManager quản lý SelectedPlot
    // ThuHoach gọi trực tiếp PlotController
    public void GieoHat(PlantData plantData)
    {
        selectedPlot.GieoHat(plantData);
        selectedPlot = null; 
    }

}
