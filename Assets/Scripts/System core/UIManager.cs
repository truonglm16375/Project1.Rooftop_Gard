using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{

    [Header("Optional: Panel mở sẵn khi vào game")]
    [SerializeField] private BaseUIPanel initialPanel;

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
    }

    private void Update()
    {
        // Bấm ESC để đóng cửa sổ trên cùng
        if (Input.GetKeyDown(KeyCode.Escape) && panelStack.Count > 0)
            CloseTopPanel();
    }

    public void OpenPanel(BaseUIPanel panel, bool hidePrevious = false)
    {
        if (panel == null || panel.IsOpen) return;
        if (hidePrevious && panelStack.Count > 0) panelStack.Peek().Hide(false);
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
    }

    public void CloseAll()
    {
        while (panelStack.Count > 0)
        {
            panelStack.Pop().Hide(false);
        }
    }
}