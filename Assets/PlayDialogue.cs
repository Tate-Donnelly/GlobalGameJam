using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// NOTE: THIS SCRIPT IS JUST FOR TESTING. I EXPECT US TO USE SOME OTHER SYSTEM
/// </summary>
public class PlayDialogue : MonoBehaviour
{
    public Text dialogueText;

    private AudioSource dlgAudioSource;

    private void Awake()
    {
        dlgAudioSource = GetComponent<AudioSource>();

        if (dialogueText.text != null) dialogueText.text = "";
    }

    private void RunDialogue(string dlgString, AudioClip dlgClip)
    {
        dialogueText.text = dlgString;
        dlgAudioSource.clip = dlgClip;
    }
}
