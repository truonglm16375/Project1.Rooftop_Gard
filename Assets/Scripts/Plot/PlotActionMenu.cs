using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlotActionMenu : MonoBehaviour 
{
    [Header("Tham chiếu cốt lõi")]
    [SerializeField] private PlotController plotController;
    [SerializeField] private SeedMenuUI seedMenu;

    [Header("Các nút chức năng")]
    [SerializeField] private Button gieoHatB;
    [SerializeField] private Button tuoiNuocB;
    [SerializeField] private Button thuHoachB;
    [SerializeField] private Button donDepB;

    [Header("Tùy chọn hiển thị")]
    [Tooltip("Nếu true, nút không thỏa mãn sẽ bị ẩn (SetActive false). Nếu false, nút chỉ bị mờ/vô hiệu hóa click (interactable = false)")]
    [SerializeField] private bool hideDisabledButtons = true;


    private void Awake()
    {
        if (plotController == null)
        {
            plotController = GetComponentInParent<PlotController>();
        }
    }

    private void OnEnable()
    {
        if (plotController == null)
        {
            plotController = GetComponentInParent<PlotController>();
        }

        // Đăng ký sự kiện các nút
        if (gieoHatB != null) gieoHatB.onClick.AddListener(OnGieoHatB);
        if (tuoiNuocB != null) tuoiNuocB.onClick.AddListener(OnTuoiNuocB);
        if (thuHoachB != null) thuHoachB.onClick.AddListener(OnThuHoachB);
        if (donDepB != null) donDepB.onClick.AddListener(OnDonDepB);

        // Lắng nghe khi cây đổi trạng thái để cập nhật nút tức thời
        PlantController.OnPlantStateChanged += HandlePlantStateChanged;

        // Cập nhật trạng thái bật/tắt (interactable) cho toàn bộ các nút
        UpdateButtonStates();
    }

    private void OnDisable() 
    {
        // Hủy đăng ký các nút
        if (gieoHatB != null) gieoHatB.onClick.RemoveListener(OnGieoHatB);
        if (tuoiNuocB != null) tuoiNuocB.onClick.RemoveListener(OnTuoiNuocB);
        if (thuHoachB != null) thuHoachB.onClick.RemoveListener(OnThuHoachB);
        if (donDepB != null) donDepB.onClick.RemoveListener(OnDonDepB);

        PlantController.OnPlantStateChanged -= HandlePlantStateChanged;
    }

    private void HandlePlantStateChanged(PlantState newState)
    {
        UpdateButtonStates();
    }

    // ==========================================
    // CÁC HÀM KIỂM TRA ĐIỀU KIỆN (VALIDATION)
    // ==========================================
    public bool CanPlant()
    {
        return plotController != null && !plotController.isPlanted;
    }
    public bool CanWater()
    {
        return plotController != null && plotController.isPlanted;
    }
    public bool CanHarvest()
    {
        return plotController != null && plotController.isPlanted && plotController.currentState == PlantState.Mature;
    }
    public bool CanClear()
    {
        return plotController != null && plotController.isPlanted;
    }

    // ==========================================
    // QUẢN LÝ TRẠNG THÁI BUTTON (INTERACTABLE)
    // ==========================================
    public void UpdateButtonStates()
    {
        SetButtonState(gieoHatB, CanPlant());
        SetButtonState(tuoiNuocB, CanWater());
        SetButtonState(thuHoachB, CanHarvest());
        SetButtonState(donDepB, CanClear());
    }

    private void SetButtonState(Button btn, bool isAllowed)
    {
        if (btn == null) return;
        if (hideDisabledButtons) btn.gameObject.SetActive(isAllowed);
        else btn.interactable = isAllowed;
    }

    // ==========================================
    // SỰ KIỆN CLICK VỚI RETURN SỚM (GUARD CLAUSES)
    // ==========================================

    public void OnGieoHatB()
    {
        UIManager.Instance.OpenPanel(seedMenu, true);
        plotController.DisableActionMenu();
    }

    public void OnTuoiNuocB()
    {
        Debug.Log("Đã bấm nút Tưới nước!");
        // TODO: Triển khai logic tưới nước sau này ở đây
    
        plotController.DisableActionMenu();
    }

    public void OnThuHoachB()
    {
        Debug.Log("Đã bấm nút Thu hoạch!");
        plotController.ThuHoach();
        plotController.DisableActionMenu();
    }

    public void OnDonDepB()
    {
        Debug.Log("Đã bấm nút Don Dep!");
        plotController.DonDep();
        plotController.DisableActionMenu();
    }
}
