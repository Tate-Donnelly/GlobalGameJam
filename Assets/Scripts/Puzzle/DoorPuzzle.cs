using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    [SerializeField] AudioSource source;

    private void OnDoorUnlocked(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.KEY) return;
        Debug.Log("Game End");
        source.Play();
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
