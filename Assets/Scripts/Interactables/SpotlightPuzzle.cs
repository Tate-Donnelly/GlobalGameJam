using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightPuzzle : MonoBehaviour
{
    [SerializeField] GameObject spotlights;

    private void OnSwitchTurnedOn(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.SWITCH) return;
        spotlights?.SetActive(true);
    }

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnSwitchTurnedOn;
        spotlights?.SetActive(false);
    }

    private void OnDestroy()
    {
        FlagSystem.OnFlagNotified -= OnSwitchTurnedOn;
    }
}
