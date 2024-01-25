using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// NOTE: THIS SCRIPT IS JUST FOR TESTING. I EXPECT US TO USE SOME OTHER SYSTEM
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayDialogue : MonoBehaviour
{
    public Text dialogueText;

    private float jokeTimer;
    private AudioSource dlgAudioSource;

    private void Awake()
    {
        dlgAudioSource = GetComponent<AudioSource>();

        if (dialogueText.text != null) dialogueText.text = "";
    }

    private void Update()
    {
        if(jokeTimer > 0)
        {
            jokeTimer -= Time.deltaTime;
            if (jokeTimer <= 0) dialogueText.text = "";
        }
    }

    public void RunJoke(JokeSO joke)
    {
        dlgAudioSource.Stop();
        dialogueText.text = joke.jokeString;
        dlgAudioSource.clip = joke.jokeClip;
        jokeTimer = joke.jokeDuration;
        dlgAudioSource.Play();
    }

    public void StopDialogue()
    {
        dlgAudioSource.Stop();
    }
}
