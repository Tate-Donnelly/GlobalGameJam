using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/Dialogue", order = 1)]
    public class DialogueSO : ScriptableObject
    {
        public string yarnNodeName;
        public float dialogueDuration;
        public AudioClip dialogueClip;
    }
}