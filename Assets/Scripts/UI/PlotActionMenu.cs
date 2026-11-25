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

    [Header("Tùy chọn hiển thị")]
    [Tooltip("Nếu true, nút không thỏa mãn sẽ bị ẩn (SetActive false). Nếu false, nút chỉ bị mờ/vô hiệu hóa click (interactable = false)")]
    [SerializeField] private bool hideDisabledButtons = false;


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

    // ==========================================
    // QUẢN LÝ TRẠNG THÁI BUTTON (INTERACTABLE)
    // ==========================================
    public void UpdateButtonStates()
    {
        SetButtonState(gieoHatB, CanPlant());
        SetButtonState(tuoiNuocB, CanWater());
        SetButtonState(thuHoachB, CanHarvest());
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
        // Return sớm nếu không thỏa mãn điều kiện gieo hạt
        if (!CanPlant())
        {
            Debug.LogWarning("Không thể gieo hạt: Ô đất đã có cây trồng!");
            UpdateButtonStates();
            return;
        }
        UIManager.Instance.OpenPanel(seedMenu, true);
        plotController.DisableActionMenu();
    }

    public void OnTuoiNuocB()
    {
        // Return sớm nếu không thỏa mãn điều kiện tưới nước
        if (!CanWater())
        {
            Debug.LogWarning("Không thể tưới nước: Ô đất chưa có cây trồng!");
            UpdateButtonStates();
            return;
        }

        Debug.Log("Đã bấm nút Tưới nước!");
        // TODO: Triển khai logic tưới nước sau này ở đây
        
        plotController.DisableActionMenu();
    }

    public void OnThuHoachB()
    {
        // Return sớm nếu không thỏa mãn điều kiện thu hoạch
        if (!CanHarvest())
        {
            Debug.LogWarning("Không thể thu hoạch: Cây chưa chín hoặc ô đất trống!");
            UpdateButtonStates();
            return;
        }

        Debug.Log("Đã bấm nút Thu hoạch!");
        plotController.ThuHoach();
        plotController.DisableActionMenu();
    }
}
