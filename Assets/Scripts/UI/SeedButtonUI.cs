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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(PlantData data, Action<PlantData> onClickCallback)
    {
        iconImg.sprite = data.seedIcon;
        nameText.text = data.plantName;
        button.onClick.AddListener(() => onClickCallback?.Invoke(data));
    }
}
