using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] ParticleSystem particles;

    public void InteractAction()
    {
        FlagSystem.NotifyFlag(PuzzleFlag.SWITCH);
    }
}
