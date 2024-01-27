using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DialogueAnimatorEventHandler : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetShowDialogueBool(bool value)
    {
        animator.SetBool("ShowDialogue", value);
    }
}
