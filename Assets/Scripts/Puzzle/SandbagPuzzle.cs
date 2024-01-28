using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandbagPuzzle : MonoBehaviour
{
    [SerializeField] GameObject sandbag;
    [SerializeField] private float fallSpeed;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip falling;
    [SerializeField] AudioClip crash;

    private void OnRopeCut(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.SANDBAG) return;
        StartCoroutine(StartFall(fallSpeed));
    }

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnRopeCut;
    }

    private void OnDestroy()
    {
        FlagSystem.OnFlagNotified -= OnRopeCut;
    }

    private IEnumerator StartFall(float time)
    {
        source.Play();
        yield return new WaitForEndOfFrame();
        Vector3 startingPos = sandbag.transform.position;
        Vector3 finalPos = sandbag.transform.position + new Vector3(0, -4.98f, 0);

        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            sandbag.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        source.Stop();
        source.PlayOneShot(crash, 0.6f);
    }
}
