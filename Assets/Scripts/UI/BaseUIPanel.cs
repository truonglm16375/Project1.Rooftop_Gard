using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUIPanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    protected Canvas subCanvas;
    public bool IsOpen { get; private set; }

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        subCanvas = GetComponent<Canvas>();
        // Mặc định ẩn toàn bộ khi khởi tạo, UIManager sẽ quyết định mở cái nào
        HideImmediate(false);
    }

    public virtual void Show()
    {
        if(subCanvas != null && !subCanvas.enabled) subCanvas.enabled = true;
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
        if (disableCanvas && subCanvas != null) subCanvas.enabled = false;
    }

    // Nút Close bên trong Panel nên gọi hàm này thay vì gọi thẳng Hide()
    public void Close()
    {
        UIManager.Instance.ClosePanel(this);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}