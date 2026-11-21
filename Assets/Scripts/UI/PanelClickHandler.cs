using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Component gắn lên một Panel nền (Backdrop / Click-catcher) nằm cùng cấp (sibling)
/// hoặc bao phủ phía sau giao diện chính.
/// Khi người chơi click ra ngoài giao diện (trúng panel này), sự kiện đóng/thoát sẽ được kích hoạt.
/// </summary>
public class PanelClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    [Header("Cấu hình mục tiêu đóng (Tùy chọn)")]
    [Tooltip("GameObject/Panel mục tiêu sẽ bị tắt (SetActive false) khi click ra ngoài. Để trống nếu muốn dùng Callback hoặc UnityEvent.")]
    [SerializeField] private GameObject targetToClose;

    [Header("Tùy chọn tương tác")]
    [Tooltip("Kích hoạt ngay khi vừa chạm chuột xuống (PointerDown) thay vì đợi nhả chuột (PointerClick)")]
    [SerializeField] private bool triggerOnPointerDown = false;

    [Tooltip("Cho phép bắt cả sự kiện chuột phải")]
    [SerializeField] private bool allowRightClick = false;

    [Header("Sự kiện")]
    [Tooltip("Sự kiện có thể kéo thả trực tiếp trên Inspector")]
    [SerializeField] private UnityEvent onPanelClicked;

    // Callback C# đăng ký qua mã nguồn (ví dụ từ PlotActionMenu hoặc các controller khác)
    public Action OnClickCallback;

    private void Reset()
    {
        SetupRaycastGraphic();
    }

    private void Awake()
    {
        SetupRaycastGraphic();
    }

    /// <summary>
    /// Đảm bảo luôn có Graphic (Image) trong suốt và bật raycastTarget
    /// để Unity EventSystem có thể bắt được tia chuột.
    /// </summary>
    private void SetupRaycastGraphic()
    {
        Graphic graphic = GetComponent<Graphic>();
        if (graphic == null)
        {
            // Tự động thêm Image trong suốt nếu chưa có
            Image img = gameObject.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f); // Hoàn toàn trong suốt
            img.raycastTarget = true;
        }
        else
        {
            graphic.raycastTarget = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!triggerOnPointerDown) return;

        if (IsValidClick(eventData))
        {
            ExecuteClickAction();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (triggerOnPointerDown) return;

        if (IsValidClick(eventData))
        {
            ExecuteClickAction();
        }
    }

    private bool IsValidClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) return true;
        if (allowRightClick && eventData.button == PointerEventData.InputButton.Right) return true;
        return false;
    }

    /// <summary>
    /// Kích hoạt chuỗi hành động đóng/thoát khi click ngoài
    /// </summary>
    public void ExecuteClickAction()
    {
        // 1. Tự động tắt target nếu được cấu hình
        if (targetToClose != null)
        {
            targetToClose.SetActive(false);
        }

        // 2. Kích hoạt C# Action delegate
        OnClickCallback?.Invoke();

        // 3. Kích hoạt UnityEvent trên Inspector
        onPanelClicked?.Invoke();
    }
}
