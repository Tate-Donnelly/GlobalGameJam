using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicNotDetected : MonoBehaviour
{
    public Image micNotDetectedImage;

    private void Start()
    {
        InvokeRepeating("CheckIfHasMic", 0, 3f);
    }

    private void CheckIfHasMic()
    {
        if (micNotDetectedImage) micNotDetectedImage.enabled = Microphone.devices.Length == 0;

    }
}
