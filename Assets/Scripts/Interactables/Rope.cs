using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour, IInteractable
{
    [SerializeField] AudioSource ropeAudio;

    public void InteractAction(ToolType type)
    {
        if (type != ToolType.KNIFE) return;
        ropeAudio.PlayOneShot(ropeAudio.clip);
        StartCoroutine(NotifyFlag());
    }

    IEnumerator NotifyFlag()
    {
        yield return new WaitForSeconds(2.5f);
        FlagSystem.NotifyFlag(PuzzleFlag.SANDBAG);
    }
}
