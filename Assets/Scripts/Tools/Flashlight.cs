using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Tool
{
    private bool flashlightOn = false;

    private void Start()
    {
        this.type = ToolType.FLASHLIGHT;
    }

    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.FLASHLIGHT) return;
        this.gameObject.SetActive(!flashlightOn);
        flashlightOn = !flashlightOn;
    }
}
