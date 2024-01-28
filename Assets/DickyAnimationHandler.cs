using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DickyAnimationHandler : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite talkSprite;
    public Sprite madSprite;
    public Sprite knockedOutSprite;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
}
