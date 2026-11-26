using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{

    [Header("Optional: Panel mở sẵn khi vào game")]
    [SerializeField] private BaseUIPanel initialPanel;

    [Header("Cài đặt background blocker")]
    [SerializeField] private GameObject blocker;
    [SerializeField] private Button blockerButton;

    [Header("Main canvas")]
    [SerializeField] private GameObject staticCanvas;
    [SerializeField] private GameObject dynamicCanvas;
    [SerializeField] private GameObject popupCanvas;

    private readonly Stack<BaseUIPanel> panelStack = new Stack<BaseUIPanel>();

    
    private UIManager() {}
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (initialPanel != null) OpenPanel(initialPanel);
        blocker.SetActive(false);
        blockerButton.onClick.AddListener(CloseTopPanel);
        staticCanvas.SetActive(true);
        dynamicCanvas.SetActive(true);
        popupCanvas.SetActive(true);
    }

    private void Update()
    {
        // Bấm ESC để đóng cửa sổ trên cùng
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseTopPanel();
        }
    }

    public void OpenPanel(BaseUIPanel panel, bool hidePrevious = false)
    {
        if (panel == null || panel.IsOpen) return;
        if (hidePrevious && panelStack.Count > 0) {
            BaseUIPanel topPanel = panelStack.Pop();
            topPanel.Hide(false);
        }
        panel.Show();
        panelStack.Push(panel);
    }

    public void CloseTopPanel()
    {
        if (panelStack.Count == 0) return;
        BaseUIPanel topPanel = panelStack.Pop();
        topPanel.Hide(false);
        if (panelStack.Count > 0 && !panelStack.Peek().IsOpen)
        {
            panelStack.Peek().Show();
        }
    }

    public void ClosePanel(BaseUIPanel panel)
    {
        if (panelStack.Count == 0) return;
        if (panelStack.Peek() == panel) CloseTopPanel();
        else
        {
            // Trường hợp hiếm: đóng một panel nằm kẹt ở giữa stack
            panel.Hide(false);
            var temp = new Stack<BaseUIPanel>();
            while (panelStack.Count > 0)
            {
                var p = panelStack.Pop();
                if (p != panel) temp.Push(p);
            }
            while (temp.Count > 0) panelStack.Push(temp.Pop());
        }

        if (panelStack.Count > 0)
        {
            UpdateBlockerBehind(panelStack.Peek());
        }
        else
        {
            blocker.SetActive(false);
        }
    }

    private void UpdateBlockerBehind(BaseUIPanel targetPanel)
    {
        blocker.SetActive(true);
        // Bước 1: Ép panel mục tiêu lên vị trí cuối cùng (trên cùng màn hình)
        targetPanel.transform.SetAsLastSibling();
        // Bước 2: Đặt Blocker nằm ngay trước panel mục tiêu 1 bậc
        int targetIndex = targetPanel.transform.GetSiblingIndex();
        blocker.transform.SetSiblingIndex(targetIndex - 1);
    }

    public void CloseAll()
    {
        while (panelStack.Count > 0)
        {
            panelStack.Pop().Hide(false);
        }
    }
}