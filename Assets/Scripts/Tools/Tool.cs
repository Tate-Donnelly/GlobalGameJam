using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    HAND,
    KNIFE,
    FLASHLIGHT,
    KEY
}

public abstract class Tool : MonoBehaviour
{
    public ToolType type;

    public abstract void InteractAction(ToolType type);
}
