using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Reactions", menuName = "Dialogue/ReactionGroup", order = 1)]
    public class ReactionGroupSO:ScriptableObject
    {
        public List<DialogueSO> reactionDialogue;

        public DialogueSO GetReaction(int index)
        {
            return reactionDialogue[index];
        }
    }
}