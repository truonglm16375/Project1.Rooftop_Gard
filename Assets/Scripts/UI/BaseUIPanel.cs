using UnityEngine;

public abstract class BaseUIPanel : MonoBehaviour
{
    [Header("UI Panel Settings")]
    [SerializeField] private bool isActiveOnStart = false;

    protected virtual void Awake()
    {
        if (!isActiveOnStart)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }
}