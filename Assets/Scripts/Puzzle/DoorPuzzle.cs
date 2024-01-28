using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    private void OnDoorUnlocked(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.KEY) return;
        
        Debug.Log("Game End");
    }

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnDoorUnlocked;
    }

    private void OnDestroy()
    {
        FlagSystem.OnFlagNotified -= OnDoorUnlocked;
    }
}
