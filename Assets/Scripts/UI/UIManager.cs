using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Inventory Slots")]
    [SerializeField] List<Image> inventorySlots;
    [SerializeField] Color INACTIVESLOTCOLOR;
    [SerializeField] Color ACTIVESLOTCOLOR;

    [SerializeField] List<string> spriteKeys;
    [SerializeField] List<Sprite> sprites;
    Dictionary<string, Sprite> inventorySprites = new();

    [Header("Interactable Display")]
    [SerializeField] TextMeshProUGUI interactablesText;
    private const string DEFAULT_TEXT = "";

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < spriteKeys.Count; i++)
        {
            inventorySprites.Add(spriteKeys[i], sprites[i]);
        }
    }

    public void HighlightInventory(int index)
    {
        foreach (Image slot in inventorySlots)
        {
            slot.color = INACTIVESLOTCOLOR;
        }
        inventorySlots[index].color = ACTIVESLOTCOLOR;
    }

    public void UpdateSlots(string name, int index)
    {
        inventorySlots[index].sprite = inventorySprites[name];
        HighlightInventory(index);
    }

    public void DisplayInteractable(string name)
    {
        interactablesText.text = name;
    }
}
