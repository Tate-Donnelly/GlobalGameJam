using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorPuzzle : MonoBehaviour
{
    [SerializeField] AudioSource source;

    private void OnDoorUnlocked(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.KEY) return;
        
        Debug.Log("Game End");
        source.Play();
        StartCoroutine(LoadMenu());
    }

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnDoorUnlocked;
    }

    private void OnDestroy()
    {
        FlagSystem.OnFlagNotified -= OnDoorUnlocked;
    }

    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
}
