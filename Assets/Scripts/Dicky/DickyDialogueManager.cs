using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using Yarn.Unity;
using Random=System.Random;

namespace Dicky
{
    public class DickyDialogueManager:MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LaughDetection _laughDetection;
        [SerializeField] private DialogueRunner _dialogueRunner;
        
        [Header("Dialogue")]
        [SerializeField] private SerializedDictionary<Reaction, Queue<DialogueSO>> _reactionsToPlayer =
            new SerializedDictionary<Reaction, Queue<DialogueSO>>();
        [SerializeField] private List<JokeSO> _unusedJokes = new List<JokeSO>();
        [SerializeField] private List<JokeSO> _usedJokes = new List<JokeSO>();
        
        //Internal
        private AudioSource dlgAudioSource;
        private JokeSO _currentJokeData;
        private float dialogueTimer;
        private float laughterTimer;
        private bool didLaugh;
        
        
        private void Awake()
        {
            dlgAudioSource = GetComponent<AudioSource>();
            
            //Event Subscribing
            _laughDetection.onLaughedDuringPunchline.AddListener(delegate { HeardLaughter(); });
            _laughDetection.onLaughedOutsidePunchline.AddListener(delegate { ReactToPlayer(Reaction.LaughedOutsideOfPunchline); });
        }

        private void Update()
        {
            if (laughterTimer > 0)
            {
                laughterTimer -= Time.deltaTime;
                if (laughterTimer <= 0)
                {
                    if(!didLaugh) ReactToPlayer(Reaction.NoLaughter);
                }
            }else if(dialogueTimer > 0)
            {
                dialogueTimer -= Time.deltaTime;
                if (dialogueTimer <= 0) EndDialogueClip();
            }
        }

        public void HeardLaughter()
        {
            laughterTimer = 0;
            PlayJoke();
        }

        private void PlayJoke()
        {
            GetRandomJoke();
            
            _dialogueRunner.StartDialogue(_currentJokeData.yarnNodeName);
            dlgAudioSource.clip=_currentJokeData.dialogueClip;
            dialogueTimer = _currentJokeData.dialogueDuration;
            _laughDetection.RunJoke(_currentJokeData);
        }

        /// <summary>
        /// Set a random joke from _unusedJokes to _currentJokeData.
        /// If there are no unused jokes left, set the list to _usedJokes
        /// </summary>
        private void GetRandomJoke()
        {
            if (_unusedJokes.Count == 0)
            {
                _unusedJokes = _usedJokes;
                _usedJokes.Clear();
            }
            
            Random rand = new Random();
            int index = rand.Next(0, _unusedJokes.Count);
            _currentJokeData = _unusedJokes[index];
            _unusedJokes.Remove(_currentJokeData);
            _usedJokes.Add(_currentJokeData);
        }

        [YarnCommand("Punchline")]
        private void WaitForLaughter()
        {
            _laughDetection.DeliveredPunchline();
        }
        
        public void ReactToPlayer(Reaction reaction)
        {
            dlgAudioSource.Stop();
            _dialogueRunner.Stop();
            //Get dialogue based on reaction
        }

        private void EndDialogueClip()
        {
            dlgAudioSource.Stop();
            _dialogueRunner.Stop();
            PlayJoke();
        }
    }

    public enum Reaction
    {
        NoLaughter,
        LaughedOutsideOfPunchline
    }
}