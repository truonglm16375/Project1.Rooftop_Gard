using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [Header("UI Reference")]
    [SerializeField] private GameObject actionMenuCanvas; 
    [SerializeField] private GameObject seedSelectionMenu;

    [Header("Hover Settings")]
    [Tooltip("Delay in seconds before hiding the menu to allow the cursor to travel between the plot and menu.")]
    [SerializeField] private float hideDelay = 0.2f;

    [Header("Plant Settings")]
    [SerializeField] private GameObject plantSpawnPoint;

    // Event
    

    // State tracking
    private bool isHoveringPlot = false;
    public bool isHoveringMenu { get; private set; } = false;
    private Coroutine hideCoroutine;

    private void Start() 
    {
        if (actionMenuCanvas != null) 
            actionMenuCanvas.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        isHoveringPlot = true;
        StopHideCoroutine();

        if (actionMenuCanvas != null && !actionMenuCanvas.activeSelf) 
            actionMenuCanvas.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        isHoveringPlot = false;
        StartHideCoroutine();
    }

    public void OnMenuEnter() 
    {
        isHoveringMenu = true;
        StopHideCoroutine();
    }

    public void OnMenuExit() 
    {
        isHoveringMenu = false;
        StartHideCoroutine();
    }

    private void StartHideCoroutine() 
    {
        StopHideCoroutine();
        hideCoroutine = StartCoroutine(CheckHideMenu());
    }

    private void StopHideCoroutine() 
    {
        if (hideCoroutine != null) 
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private IEnumerator CheckHideMenu() 
    {
        yield return new WaitForSeconds(hideDelay);
        if (!isHoveringPlot && !isHoveringMenu && actionMenuCanvas != null) 
        {
            actionMenuCanvas.SetActive(false);
            isHoveringMenu = false;
        }
        hideCoroutine = null;
    }

    public void OnPlantButton()
    {
        Debug.Log("Đã bấm nút Trồng cây!");
        Vector3 plantPosition = plantSpawnPoint.transform.position; // Di chuyển cây đến vị trí spawn
        if (seedSelectionMenu != null)
        {
            seedSelectionMenu.SetActive(true);
        }
        /*
        private Plant plantPrefab = 
        GameObject selectedPlant = Instantiate(plantPrefab, plantPosition, Quaternion.identity);

        */
    }
}
