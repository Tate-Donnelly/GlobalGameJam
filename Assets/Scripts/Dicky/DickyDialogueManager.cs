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
        [SerializeField] private AudioSource dlgAudioSource;
        private DialogueSO _currentDialogueData;
        private float dialogueTimer;
        private float laughterTimer;
        private bool didLaugh;
        
        
        private void Awake()
        {
            //Event Subscribing
            _laughDetection.onLaughedDuringPunchline.AddListener(delegate
            {
                print("HeardLaughter");
                HeardLaughter();
            });
            _laughDetection.onLaughedOutsidePunchline.AddListener(delegate
            {
                print("ReactToPlayer");
                ReactToPlayer(Reaction.LaughedOutsideOfPunchline);
            });

            _dialogueRunner.AddCommandHandler("Punchline", (GameObject target) => { Punchline(); });

            PlayJoke();
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

        public void PlayJoke()
        {
            GetRandomJoke();
            
            _dialogueRunner.StartDialogue(_currentDialogueData.yarnNodeName);
            dlgAudioSource.clip=_currentDialogueData.dialogueClip;
            dialogueTimer = _currentDialogueData.dialogueDuration;
            
            _laughDetection.RunJoke(_currentDialogueData as JokeSO);
            dlgAudioSource.Play();
        }

        /// <summary>
        /// Set a random joke from _unusedJokes to _currentJokeData.
        /// If there are no unused jokes left, set the list to _usedJokes
        /// </summary>
        private void GetRandomJoke()
        {
            Random rand = new Random();
            int index = rand.Next(0, _unusedJokes.Count);
            _currentDialogueData = _unusedJokes[index];
            if (_unusedJokes.Count == 1)
            {
                _unusedJokes.AddRange(_usedJokes);
                _usedJokes.Clear();
            }
        }

        [YarnCommand("Punchline")]
        private void Punchline()
        {
            _laughDetection.punchlineDelivered = true;
        }
        
        public void ReactToPlayer(Reaction reaction)
        {
            
            dlgAudioSource.Stop();
            _dialogueRunner.Stop();
            //Get dialogue based on reaction
            _currentDialogueData = _reactionsToPlayer[reaction].Dequeue();
        }

        private void EndDialogueClip()
        {
            _unusedJokes.Remove(_currentDialogueData as JokeSO);
            _usedJokes.Add(_currentDialogueData as JokeSO);
            if (_unusedJokes.Count == 0)
            {
                _unusedJokes = _usedJokes;
                print("New count: "+_unusedJokes.Count);
                _usedJokes.Clear();
            }
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