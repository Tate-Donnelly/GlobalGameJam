using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] Sprite onState;
    [SerializeField] Material onStateDiffuse;
    [SerializeField] AudioSource spotlightOnClip;

    public void InteractAction(ToolType type)
    {
        if (type != ToolType.HAND) return;
        particles.Play();
        this.GetComponent<SpriteRenderer>().sprite = onState;
        this.GetComponent<SpriteRenderer>().material = onStateDiffuse;
        spotlightOnClip.PlayOneShot(spotlightOnClip.clip);
        FlagSystem.NotifyFlag(PuzzleFlag.SWITCH);
    }
}
