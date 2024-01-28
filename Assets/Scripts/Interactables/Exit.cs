using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour, IInteractable
{
    public void InteractAction(ToolType type)
    {
        if (type != ToolType.KEY) return;
        FlagSystem.NotifyFlag(PuzzleFlag.KEY);
    }
}
