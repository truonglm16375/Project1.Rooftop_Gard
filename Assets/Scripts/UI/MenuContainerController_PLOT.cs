using UnityEngine;
using UnityEngine.EventSystems;

public class MenuContainerController_PLOT : BaseUIPanel, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] private PlotController plotController;

    override protected void Awake() 
    {
        base.Awake();
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        Debug.Log("MenuContainerController_PLOT: OnPointerEnter");
        if (plotController != null) 
        {
            plotController.OnMenuEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        if (plotController != null) 
        {
            plotController.OnMenuExit();
        }
    }

    private void OnDisable() 
    {
        if (plotController != null) 
        {
            plotController.OnMenuExit();
        }
    }
}
