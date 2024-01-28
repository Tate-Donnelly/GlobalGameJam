﻿using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using Yarn;
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
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<Reaction, DickyReaction> _reactionsToPlayer=new AYellowpaper.SerializedCollections.SerializedDictionary<Reaction, DickyReaction>();
        [SerializeField] private List<JokeGroup> _unusedJokes = new List<JokeGroup>();
        [SerializeField] private List<JokeGroup> _usedJokes = new List<JokeGroup>();
        
        //Internal
        private Queue<Reaction> reactionQue = new Queue<Reaction>();
        private DialogueSO _currentDialogueData;
        private JokeGroup _currentJokeGroup;
        private float dialoguePauseTimer;
        private float dialogueTimer;
        private float laughterTimer;
        private bool didLaugh;
        private bool stopTellingJokes;
        private bool spotlightOn;
        private bool sandbagFell;
        
        private void Start()
        {
            //Event Subscribing
            _laughDetection.onLaughedDuringPunchline.AddListener(delegate
            {
                HeardLaughter();
            });
            _laughDetection.onLaughedOutsidePunchline.AddListener(delegate
            {
                QueueReaction(Reaction.LAUGHED_BEFORE_PUNCHLINE);
            });
            FlagSystem.instance.OnFlagNotifiedUnityEvent.AddListener(FlagUpdate);

            _dialogueRunner.AddCommandHandler("Punchline", (GameObject target) => { Punchline(); });
            _dialogueRunner.AddCommandHandler("PlayRandomJoke", (GameObject target) => { PlayRandomJoke(); });

            //PlayRandomJoke();
            _dialogueRunner.StartDialogue("Intro");
        }

        private void Update()
        {
            if(stopTellingJokes) return;
            //if (dialoguePauseTimer > 0)
            //{
            //    dialoguePauseTimer -= Time.deltaTime;
            //    if (dialoguePauseTimer <= 0) PlayRandomJoke();
            //}
        }

        #region Play Dialogue

        /// <summary>
        /// Set a random joke from _unusedJokes to _currentJokeData.
        /// If there are no unused jokes left, set the list to _usedJokes
        /// </summary>
        [YarnCommand("PlayRandomJoke")]
        private void PlayRandomJoke()
        {
            Random rand = new Random();
            int index = rand.Next(0, _unusedJokes.Count);
            _currentJokeGroup = _unusedJokes[index];

            JokeSO jokeData = _currentJokeGroup.GetNextJoke();
            _laughDetection.RunJoke(jokeData);
            PlayDialogue(jokeData);
            StartCoroutine(WaitForJokeToEnd());
            
            laughterTimer = jokeData.dialogueDuration;
            if (_unusedJokes.Count == 1)
            {
                _unusedJokes.AddRange(_usedJokes);
                _usedJokes.Clear();
            }
        }

        private void PlayNextGroupJoke(JokeSO joke)
        {
            PlayDialogue(joke);
            StartCoroutine(WaitForJokeToEnd());
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
            QueueReaction(Reaction.LAUGHED_DURING_PUNCHLINE);
        }

        [YarnCommand("Punchline")]
        private void Punchline()
        {
            _laughDetection.RunPunchline();
        }
        
        private IEnumerator WaitForJokeToEnd()
        {
            yield return new WaitUntil(() => !_dialogueRunner.IsDialogueRunning);
            _unusedJokes.Remove(_currentJokeGroup);
            _usedJokes.Add(_currentJokeGroup);
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
            if (!_laughDetection.hasLaughed) reaction = Reaction.NO_LAUGHTER;
            else
            {
                reaction = reactionQue.Dequeue();
                print(reaction.ToString());
                reactionQue.Clear();
            }
            
            _currentDialogueData = _reactionsToPlayer[reaction].GetReaction();
            PlayDialogue(_currentDialogueData);
            yield return new WaitUntil(() => !_dialogueRunner.IsDialogueRunning);
            // TODO: Check Flags for player death
            if(stopTellingJokes) StopDialogue();

            if (_currentJokeGroup.HasMoreJokes())
            {
                PlayNextGroupJoke(_currentJokeGroup.GetNextJoke());
            }
            else
            {
                PlayRandomJoke();
            }
        }

        private void FlagUpdate(FlagArgs flagArgs)
        {
            switch (flagArgs.flag)
            {
                case PuzzleFlag.SWITCH:
                    // Player turns on spotlight
                    QueueReaction(Reaction.SPOTLIGHT_ON);
                    spotlightOn = true;
                    return;
                case PuzzleFlag.UNTIE:
                    // Player unties self
                    if(!spotlightOn) QueueReaction(Reaction.PLAYER_CAUGHT_UNTYING);
                    return;
                case PuzzleFlag.SANDBAG:
                    CutOffDialogue();
                    sandbagFell = true;
                    return;
                case PuzzleFlag.PLAYER_DEATH:
                    CutOffDialogue();
                    return;
                case PuzzleFlag.KEY:
                    // Player gets Key
                    if(!sandbagFell) QueueReaction(Reaction.PLAYER_CAUGHT_BY_STAGE);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        INTRO,
        SPOTLIGHT_ON,
        LAUGHED_DURING_PUNCHLINE,
        NO_LAUGHTER,
        LAUGHED_BEFORE_PUNCHLINE,
        PLAYER_CAUGHT_UNTYING,
        PLAYER_CAUGHT_BY_STAGE,
        PLAYER_CAUGHT_LEAVING,
        PLAYER_CAUGHT_FLASHLIGHT
    }
}