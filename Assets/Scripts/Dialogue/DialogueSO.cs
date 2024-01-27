using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/dialogue", order = 1)]
    public class DialogueSO : ScriptableObject
    {
        public string yarnNodeName;
        public float dialogueDuration;
        public AudioClip dialogueClip;
    }
}