using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUIPanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    protected Canvas canvas;
    public bool IsOpen { get; private set; }

    [Header("Base UI Settings")]
    [SerializeField] protected Button closeButton; 
    // Để trống nếu UI này không có nút tắt
    

    protected virtual void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();

        // Mặc định ẩn toàn bộ khi khởi tạo, UIManager sẽ quyết định mở cái nào
        HideImmediate(false);
    }

    public virtual void Show()
    {
        if(canvas != null && !canvas.enabled) canvas.enabled = true;
        IsOpen = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        transform.SetAsLastSibling(); // Đưa Panel này lên lớp hiển thị trên cùng
        OnShow();
    }

    public virtual void Hide(bool disableCanvas)
    {
        HideImmediate(disableCanvas);
        OnHide();
    }

    private void HideImmediate(bool disableCanvas)
    {
        IsOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (disableCanvas && canvas != null) canvas.enabled = false;
    }

    protected void OnCloseButtonClicked()
    {
        Hide(false);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }

    protected virtual void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}