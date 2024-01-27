using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Inventory Slots")]
    [SerializeField] List<Image> inventorySlots;
    [SerializeField] Color INACTIVESLOTCOLOR;
    [SerializeField] Color ACTIVESLOTCOLOR;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this);
        else
            Instance = this;
    }

    public void HighlightInventory(int index)
    {
        
        foreach (Image slot in inventorySlots)
        {
            slot.color = INACTIVESLOTCOLOR;
        }
        inventorySlots[index].color = ACTIVESLOTCOLOR;
    }
}
