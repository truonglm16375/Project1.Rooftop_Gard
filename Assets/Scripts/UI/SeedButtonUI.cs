using UnityEngine;
using UnityEngine.UI; // for Button & Image datatype
using TMPro; // for TextMeshProUGUI datatype
using System; // for Action 
public class SeedButtonUI : MonoBehaviour
{   
    private PlantData plantData;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void SetUp(PlantData data)
    {
        // Gán giá trị cho thuộc tính của button
        iconImg.sprite = data.icon;
        nameText.text = data.Name;
        plantData = data;

        // Khi click sẽ gọi hàm gieo hạt, và đóng seedMenu qua UIManager
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(GieoHat);
        var seedMenu = GetComponentInParent<SeedMenuUI>();
        button.onClick.AddListener(() => UIManager.Instance.ClosePanel(seedMenu));
    }

    // Chỉ định PlotManager thực hiện Gieo hạt
    private void GieoHat()
    {
        PlotManager.Instance.GieoHat(plantData);
    }
}
