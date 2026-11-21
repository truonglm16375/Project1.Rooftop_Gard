using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
    public List<ItemStack> item = new List<ItemStack>(); 
    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Inventory>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("Inventory");
                    _instance = go.AddComponent<Inventory>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        
    }
}
