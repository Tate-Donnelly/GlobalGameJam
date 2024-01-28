using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DickyAnimationHandler : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite talkSprite;
    public Sprite madSprite;
    public Sprite knockedOutSprite;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private DialogueRunner dialogueRunner; // Yarn Spinner Dialogue Runner

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if(dialogueRunner != null) dialogueRunner = FindObjectOfType<DialogueRunner>();
        SetupYarnCommands();
    }

    private void SetupYarnCommands()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.AddCommandHandler("ChangeSprite", (string spriteString) => ChangeSprite(spriteString));
        dialogueRunner.AddCommandHandler("PlayEffect", (string effectString) => ChangeSprite(effectString));
        dialogueRunner.AddCommandHandler("PlayStatus", (string statusString) => ChangeSprite(statusString));
    }

    //Using strings here for yarnspinner
    public void ChangeSprite(string spriteString)
    {
        switch(spriteString)
        {
            case "Idle":
                spriteRenderer.sprite = idleSprite;
                break;
            case "Talk":
                spriteRenderer.sprite = talkSprite;
                break;
            case "Mad":
                spriteRenderer.sprite = madSprite;
                break;
            case "KnockedOut":
                spriteRenderer.sprite = knockedOutSprite;
                break;
        }
    }

    public void PlayEffect(string effectString)
    {
        switch (effectString)
        {
            case "Still":
                anim.Play("Dicky_Still");
                break;
            case "Jump":
                anim.Play("Dicky_Jump");
                break;
            case "GrowShrink":
                anim.Play("Dicky_GrowShrink");
                break;
            case "Sway":
                anim.Play("Dicky_Sway");
                break;
            case "Shrink":
                anim.Play("Dicky_Shrink");
                break;
            case "Jump_Repeat":
                anim.Play("Dicky_JumpRepeat");
                break;
        }
    }

    public void PlayStatus(string statusString)
    {
        switch (statusString)
        {
            case "Question":
                anim.Play("Status_Question_White");
                break;
            case "Question_Red":
                anim.Play("Status_Question_Red");
                break;
            case "Exclaim":
                anim.Play("Status_Exclaim");
                break;
            case "Exclaim_Green":
                anim.Play("Status_Exclaim_Green");
                break;
            case "Exclaim_Red":
                anim.Play("Status_Exclaim_Red");
                break;
        }
    }

    public void PlayQuestionBasic()
    {
        anim.Play("Status_Question_White");
    }

    public void PlayQuestionRed()
    {
        anim.Play("Status_Question_Red");
    }

    public void PlayExclamationPoint()
    {
        anim.Play("Status_Exclaim");
    }

    public void PlayExclamationPointGreen()
    {
        anim.Play("Status_Exclaim_Green");
    }

    public void PlayExclamationPointRed()
    {
        anim.Play("Status_Exclaim_Red");
    }
}
