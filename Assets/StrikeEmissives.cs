using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class StrikeEmissives : MonoBehaviour
{
    [SerializeField] private DialogueRunner _dialogueRunner;
    private int strikes = 0;

    [SerializeField] List<GameObject> strikeEmissives;

    private void Awake()
    {
        _dialogueRunner.AddCommandHandler("Strikes", (GameObject target) => { StrikeReceived(); });
    }

    void StrikeReceived()
    {
        strikeEmissives[strikes].SetActive(true);
        strikes++;
    }
}
