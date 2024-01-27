using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagTest : MonoBehaviour
{
    private void OnFlagNotified(object sender, FlagArgs e) {
        gameObject.SetActive(false);
    }

    public void FlagNotified(FlagArgs e) {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Awake()
    {
        FlagSystem.OnFlagNotified += OnFlagNotified;   
    }
}
