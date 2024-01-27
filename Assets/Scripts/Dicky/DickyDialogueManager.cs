using System.Collections;
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
        [SerializeField] private AudioSource dlgAudioSource;
        [SerializeField] private float timeBetweenJokes;

        [Header("Dialogue")] 
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<Reaction, ReactionGroupSO> _reactionsToPlayer=new AYellowpaper.SerializedCollections.SerializedDictionary<Reaction, ReactionGroupSO>();
        [SerializeField] private List<JokeSO> _unusedJokes = new List<JokeSO>();
        [SerializeField] private List<JokeSO> _usedJokes = new List<JokeSO>();
        
        //Internal
        private DialogueSO _currentDialogueData;
        private float dialoguePauseTimer;
        private float dialogueTimer;
        private float laughterTimer;
        private bool didLaugh;
        private bool stopTellingJokes;
        
        
        private void Awake()
        {
            //Event Subscribing
            _laughDetection.onLaughedDuringPunchline.AddListener(delegate
            {
                HeardLaughter();
            });
            _laughDetection.onLaughedOutsidePunchline.AddListener(delegate
            {
                print("Laughed too early");
                StartCoroutine(WaitForJokeToEnd(Reaction.LaughedOutsideOfPunchline));
            });

            _dialogueRunner.AddCommandHandler("Punchline", (GameObject target) => { Punchline(); });

            PlayRandomJoke();
        }

        private void Update()
        {
            if(stopTellingJokes) return;
            if (dialoguePauseTimer > 0)
            {
                dialoguePauseTimer -= Time.deltaTime;
                if (dialoguePauseTimer <= 0) PlayRandomJoke();
            }else if (laughterTimer > 0)
            {
                laughterTimer -= Time.deltaTime;
                if (laughterTimer <= 0)
                {
                    if (!didLaugh) StartCoroutine(ReactToPunchlineResponse(Reaction.NoLaughter));
                }
            }
        }

        #region Play Dialogue

        /// <summary>
        /// Set a random joke from _unusedJokes to _currentJokeData.
        /// If there are no unused jokes left, set the list to _usedJokes
        /// </summary>
        private void PlayRandomJoke()
        {
            Random rand = new Random();
            int index = rand.Next(0, _unusedJokes.Count);
            PlayDialogue(_unusedJokes[index]);
            laughterTimer = _unusedJokes[index].dialogueDuration;
            if (_unusedJokes.Count == 1)
            {
                _unusedJokes.AddRange(_usedJokes);
                _usedJokes.Clear();
            }
        }

        private void PlayDialogue(DialogueSO dialogueSo)
        {
            if (_dialogueRunner.IsDialogueRunning) return;
            _currentDialogueData = dialogueSo;
            _dialogueRunner.StartDialogue(dialogueSo.yarnNodeName);
            dlgAudioSource.clip=dialogueSo.dialogueClip;
            dlgAudioSource.loop = true;
            dialogueTimer = dialogueSo.dialogueDuration;
            
            dlgAudioSource.Play();
        }
        #endregion


        #region React to Player
        
        private void HeardLaughter()
        {
            if (laughterTimer == 0) return;
            laughterTimer = 0;
            StartCoroutine(WaitForJokeToEnd(Reaction.LaughedDuringPunchline));
        }

        [YarnCommand("Punchline")]
        private void Punchline()
        {
            _laughDetection.RunPunchline(_currentDialogueData as JokeSO);
        }
        
        private IEnumerator WaitForJokeToEnd(Reaction reaction)
        {
            yield return new WaitUntil(() => !_dialogueRunner.IsDialogueRunning);
            _unusedJokes.Remove(_currentDialogueData as JokeSO);
            _usedJokes.Add(_currentDialogueData as JokeSO);
            if (_unusedJokes.Count == 0)
            {
                _unusedJokes = _usedJokes;
                _usedJokes.Clear();
            }

            StopDialogue();
            
            StartCoroutine(ReactToPunchlineResponse(reaction));
        }

        private IEnumerator ReactToPunchlineResponse(Reaction reaction)
        {
            print("React");
            //Get dialogue based on reaction
            //TODO Check flags for appropriate reaction
            _currentDialogueData = _reactionsToPlayer[reaction].GetReaction(0);
            PlayDialogue(_currentDialogueData);
            yield return new WaitUntil(() => !_dialogueRunner.IsDialogueRunning);
            // TODO: Check Flags for player death
            StopDialogue();
            dialoguePauseTimer = timeBetweenJokes;
        }

        private void StopDialogue()
        {
            dlgAudioSource.loop = false;
            dlgAudioSource.Stop();
            _dialogueRunner.Stop();
        }

        public void CutOffDialogue()
        {
            stopTellingJokes = true;
            dlgAudioSource.Stop();
            _dialogueRunner.Stop();
        }
        #endregion
    }

    public enum Reaction
    {
        LaughedDuringPunchline,
        NoLaughter,
        LaughedOutsideOfPunchline
    }
}