using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Tool
{
    private bool flashlightOn = false;
    [SerializeField] AudioSource flashlightSFX;

    private void Start()
    {
        this.type = ToolType.FLASHLIGHT;
    }

    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.FLASHLIGHT) return;
        flashlightOn = !flashlightOn;
        flashlightSFX.Play();
        this.gameObject.SetActive(flashlightOn);
    }
}
