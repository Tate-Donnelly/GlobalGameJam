using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour, IInteractable
{
    public void InteractAction(ToolType type)
    {
        if (!FlagSystem.FlagsNotified.Contains(PuzzleFlag.SWITCH)) {
            FlagSystem.NotifyFlag(PuzzleFlag.PLAYER_DEATH);
        }
        if (type != ToolType.KNIFE) return;
        FlagSystem.NotifyFlag(PuzzleFlag.SANDBAG);
    }
}
