using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ManualLaugh : MonoBehaviour
{
    [SerializeField] private bool allowManualLaugh = true;
    [SerializeField] private LaughDetection laughDetection;
    [SerializeField] private UnityEvent onManualLaughed;

    private void Awake()
    {
        if(laughDetection == null) laughDetection = FindObjectOfType<LaughDetection>();
    }

    public void CallManualLaugh(InputAction.CallbackContext context)
    {
        if(allowManualLaugh)
        {
            laughDetection.OnHasLaughed(new CapturedPlayerResponse(DateTime.Now));
            onManualLaughed?.Invoke();
        }
    }
}
