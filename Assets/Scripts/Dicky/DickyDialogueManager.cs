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
        private Queue<Reaction> reactionQue = new Queue<Reaction>();
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
                QueueReaction(Reaction.LaughedOutsideOfPunchline);
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
            _laughDetection.RunJoke(_unusedJokes[index]);
            PlayDialogue(_unusedJokes[index]);
            StartCoroutine(WaitForJokeToEnd());
            
            laughterTimer = _unusedJokes[index].dialogueDuration;
            if (_unusedJokes.Count == 1)
            {
                _unusedJokes.AddRange(_usedJokes);
                _usedJokes.Clear();
            }
        }

        private void PlayDialogue(DialogueSO dialogueSo)
        {
            _currentDialogueData = dialogueSo;
            _dialogueRunner.StartDialogue(dialogueSo.yarnNodeName);
            dlgAudioSource.clip=dialogueSo.dialogueClip;
            dlgAudioSource.loop = true;
            dialogueTimer = dialogueSo.dialogueDuration;
            
            dlgAudioSource.Play();
        }
        #endregion


        #region React to Player

        public void QueueReaction(Reaction reaction)
        {
            reactionQue.Enqueue(reaction);
        }
        
        private void HeardLaughter()
        {
            if (laughterTimer == 0) return;
            laughterTimer = 0;
            QueueReaction(Reaction.LaughedDuringPunchline);
        }

        [YarnCommand("Punchline")]
        private void Punchline()
        {
            _laughDetection.RunPunchline();
        }
        
        private IEnumerator WaitForJokeToEnd()
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
            
            StartCoroutine(ReactToPunchlineResponse());
        }

        private IEnumerator ReactToPunchlineResponse()
        {
            //Get dialogue based on reaction
            Reaction reaction;
            if (!_laughDetection.hasLaughed) reaction = Reaction.NoLaughter;
            else
            {
                reaction = reactionQue.Dequeue();
                reactionQue.Clear();
            }
            
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