using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsTiedPuzzle : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Sprite breakingOutSprite;
    [SerializeField] Material breakingOutDiffuse;
    [SerializeField] int health = 4;
    [SerializeField] List<AudioClip> bindTugs;
    [SerializeField] AudioSource source;

    public void InteractAction(ToolType type)
    {
        if (type != ToolType.HAND) return;
        if(health < 0)
        {
            source.PlayOneShot(source.clip);
            if (!FlagSystem.FlagsNotified.Contains(PuzzleFlag.SWITCH)) {
                FlagSystem.KillPlayer(PuzzleFlag.UNTIE);
                return;
            }
            FlagSystem.NotifyFlag(PuzzleFlag.UNTIE);
            rend.enabled = false;
            StartCoroutine(DisableHands());
            return;
        }
        else if(health < 2)
        {
            rend.sprite = breakingOutSprite;
            rend.material = breakingOutDiffuse;
        }
        source.PlayOneShot(bindTugs[health]);
        health--;
    }
    IEnumerator DisableHands()
    {
        yield return new WaitForSeconds(0.7f);
        this.gameObject.SetActive(false);
    }
}
