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
    public void SetUp(SeedData data)
    {
        // Gán giá trị cho thuộc tính của button
        iconImg.sprite = data.itemIcon;
        nameText.text = data.itemName;
        plantData = data.plantData;

        // Khi click sẽ gọi hàm gieo hạt, và đóng seedMenu qua UIManager
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => GieoHat(data));
        var seedMenu = GetComponentInParent<SeedMenuUI>();
        button.onClick.AddListener(() => UIManager.Instance.ClosePanel(seedMenu));
    }

    // Chỉ định PlotManager thực hiện Gieo hạt
    private void GieoHat(SeedData data)
    {
        if(!Inventory.Instance.SubtractItem(data, 1)) return;
        PlotManager.Instance.GieoHat(plantData);
    }
}
