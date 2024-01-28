using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] ParticleSystem particles;

    public void InteractAction(ToolType type)
    {
        if (type != ToolType.HAND) return;
        particles.Play();
        FlagSystem.NotifyFlag(PuzzleFlag.SWITCH);
    }
}
