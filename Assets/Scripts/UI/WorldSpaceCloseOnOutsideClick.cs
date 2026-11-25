using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class WorldSpaceCloseOnOutsideClick : MonoBehaviour
{
    [Tooltip("Kéo thả chính Canvas hoặc Panel chính của UI này vào đây")]
    public RectTransform uiContainer; 

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Lấy vị trí chuột theo hệ thống mới
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (IsPointerOverThisUI(mousePos)) return;
            PlotController plotController = GetComponentInParent<PlotController>();
            plotController.DisableActionMenu();
        }
    }

    private bool IsPointerOverThisUI(Vector2 mousePos)
    {
        // Tạo dữ liệu sự kiện tại vị trí chuột hiện tại
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        // Bắn tia Raycast vào tất cả các UI trên màn hình
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Kiểm tra xem có trúng bất kỳ thành phần con nào của Canvas này không
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.IsChildOf(uiContainer))
            {
                return true; // Click trúng bên trong UI
            }
        }

        return false; // Click ra ngoài
    }
}
