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
    [SerializeField] private AK.Wwise.Event laughSound;

    private void Awake()
    {
        if(laughDetection == null) laughDetection = FindObjectOfType<LaughDetection>();
    }

    public void CallManualLaugh(InputAction.CallbackContext context)
    {
        laughDetection.OnHasLaughed(new CapturedPlayerResponse(DateTime.Now));
        onManualLaughed?.Invoke();
        laughSound.Post(gameObject);
    }
}
