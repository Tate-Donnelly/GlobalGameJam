using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ManualLaugh : MonoBehaviour
{
    [SerializeField] private bool allowManualLaugh = true;
    [SerializeField] private LaughDetection laughDetection;
    [SerializeField] private UnityEvent onManualLaughed;
    [SerializeField] private List<AudioClip> laughClips = new List<AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
        if(laughDetection == null) laughDetection = FindObjectOfType<LaughDetection>();
        audioSource = GetComponent<AudioSource>();
    }

    public void CallManualLaugh(InputAction.CallbackContext context)
    {
        if(audioSource && laughClips.Count > 0) {
            if(audioSource.isPlaying) return;
        }

        if(allowManualLaugh)
        {
            laughDetection.ManualLaugh();
            onManualLaughed?.Invoke();
            if (audioSource)
            {
                audioSource.clip = laughClips[UnityEngine.Random.Range(0, laughClips.Count)];
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.15f);
                audioSource.Play();
            }
        }
    }
}
