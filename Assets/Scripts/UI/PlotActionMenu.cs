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

    [Header("Click Outside / Blocker")]
    [Tooltip("Panel bắt click ra ngoài (Sibling nằm dưới menu) để tự động đóng menu khi người chơi click ra ngoài")]
    [SerializeField] private PanelClickHandler backgroundBlocker;

    [Header("Tùy chọn hiển thị")]
    [Tooltip("Nếu true, nút không thỏa mãn sẽ bị ẩn (SetActive false). Nếu false, nút chỉ bị mờ/vô hiệu hóa click (interactable = false)")]
    [SerializeField] private bool hideDisabledButtons = false;

    public PanelClickHandler BackgroundBlocker => backgroundBlocker;

    private void Awake()
    {
        if (plotController == null)
        {
            plotController = GetComponentInParent<PlotController>();
        }
        InitBlocker();
    }

    private void OnEnable()
    {
        if (plotController == null)
        {
            plotController = GetComponentInParent<PlotController>();
        }

        // Đăng ký và bật Blocker click-outside
        InitBlocker();
        if (backgroundBlocker != null)
        {
            backgroundBlocker.OnClickCallback = OnClickOutside;
            backgroundBlocker.gameObject.SetActive(true);
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
        // Hủy đăng ký Blocker
        if (backgroundBlocker != null)
        {
            backgroundBlocker.OnClickCallback = null;
            backgroundBlocker.gameObject.SetActive(false);
        }

        // Hủy đăng ký các nút
        if (gieoHatB != null) gieoHatB.onClick.RemoveListener(OnGieoHatB);
        if (tuoiNuocB != null) tuoiNuocB.onClick.RemoveListener(OnTuoiNuocB);
        if (thuHoachB != null) thuHoachB.onClick.RemoveListener(OnThuHoachB);

        PlantController.OnPlantStateChanged -= HandlePlantStateChanged;
    }

    /// <summary>
    /// Tự động tìm Background Blocker nếu chưa được gán thủ công trong Inspector
    /// </summary>
    private void InitBlocker()
    {
        if (backgroundBlocker == null)
        {
            if (transform.parent != null)
            {
                backgroundBlocker = transform.parent.GetComponentInChildren<PanelClickHandler>(true);
            }
            if (backgroundBlocker == null)
            {
                backgroundBlocker = GetComponentInChildren<PanelClickHandler>(true);
            }
        }
    }

    private void HandlePlantStateChanged(PlantState newState)
    {
        UpdateButtonStates();
    }

    /// <summary>
    /// Được gọi khi người chơi click vào vùng Blocker phía sau menu
    /// </summary>
    private void OnClickOutside()
    {
        Debug.Log("Đã click ra ngoài menu thao tác -> Đóng PlotActionMenu.");
        if (plotController != null)
        {
            plotController.DisableActionMenu();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // ==========================================
    // CÁC HÀM KIỂM TRA ĐIỀU KIỆN (VALIDATION)
    // ==========================================

    /// <summary>
    /// Điều kiện để gieo hạt: Ô đất chưa có cây trồng
    /// </summary>
    public bool CanPlant()
    {
        return plotController != null && !plotController.isPlanted;
    }

    /// <summary>
    /// Điều kiện để tưới nước: Ô đất đã có cây (sau này có thể thêm: chưa được tưới trong ngày)
    /// </summary>
    public bool CanWater()
    {
        return plotController != null && plotController.isPlanted;
    }

    /// <summary>
    /// Điều kiện để thu hoạch: Ô đất có cây và cây đã ở giai đoạn chín (Mature)
    /// </summary>
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

        Debug.Log("Đã bấm nút Gieo hạt!");
        if (seedMenu != null)
        {
            Debug.Log("Mở SeedMenu");
            UIManager.Instance.OpenPanel(seedMenu, true);
        }

        if (plotController != null)
        {
            plotController.DisableActionMenu();
        }
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
        
        if (plotController != null)
        {
            plotController.DisableActionMenu();
        }
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
        // TODO: Triển khai logic thu hoạch sau này ở đây

        if (plotController != null)
        {
            plotController.DisableActionMenu();
        }
    }
}
