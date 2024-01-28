using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour, IInteractable
{
    public void InteractAction(ToolType type)
    {
        if (type != ToolType.KEY) return;
        // If player hasn't killed Dicky and tries to escape, Dicky kills the player
        if (!FlagSystem.FlagsNotified.Contains(PuzzleFlag.SANDBAG)) {
            FlagSystem.KillPlayer(PuzzleFlag.KEY);
            return;
        }
        
        FlagSystem.NotifyFlag(PuzzleFlag.KEY);
    }
}
