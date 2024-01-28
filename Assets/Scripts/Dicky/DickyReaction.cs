using System;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

[Serializable]
public class DickyReaction
{
    public List<DialogueSO> reactionDialogue=new List<DialogueSO>();
    private Queue<DialogueSO> reactionQue=new Queue<DialogueSO>();

    void Awake()
    {
        reactionQue = new Queue<DialogueSO>(reactionDialogue);
        foreach (DialogueSO dialogueSo in reactionDialogue)
        {
            reactionQue.Enqueue(dialogueSo);
        }
        Debug.Log(reactionDialogue.Count);
        Debug.Log(reactionQue.Count);
    }

    void LoadQue()
    {
        reactionQue = new Queue<DialogueSO>(reactionDialogue);
        foreach (DialogueSO dialogueSo in reactionDialogue)
        {
            reactionQue.Enqueue(dialogueSo);
        }
    }

    public DialogueSO GetReaction()
    {
        if (reactionQue.Count == 0) LoadQue();
        return reactionQue.Dequeue();
    }
}