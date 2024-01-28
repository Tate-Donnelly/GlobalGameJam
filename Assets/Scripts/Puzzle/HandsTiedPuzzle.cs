using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsTiedPuzzle : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Sprite breakingOutSprite;
    [SerializeField] Material breakingOutDiffuse;
    [SerializeField] int health = 4;

    public void InteractAction(ToolType type)
    {
        if (type != ToolType.HAND) return;
        if(health < 0)
        {
            FlagSystem.NotifyFlag(PuzzleFlag.UNTIE);
            this.gameObject.SetActive(false);
        }
        else if(health < 2)
        {
            rend.sprite = breakingOutSprite;
            rend.material = breakingOutDiffuse;
        }
        health--;
    }
}
