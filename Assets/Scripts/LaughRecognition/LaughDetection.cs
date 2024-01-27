using Recognissimo;
using Recognissimo.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Detects laughter from voice speech in combo with voice response.
/// Also verifies a laugh was made during the punchline time.
/// </summary>
public class LaughDetection : MonoBehaviour
{
    private readonly RecognizedText _recognizedText = new();

    [SerializeField]
    private VoiceActivityDetector activityDetector;

    [SerializeField]
    private SpeechRecognizer recognizer;

    [SerializeField]
    private StreamingAssetsLanguageModelProvider languageModelProvider;

    /*[SerializeField]
    private InputField status;

    [SerializeField]
    private Text hearStatus;*/

    [Header("Callbacks")]
    [SerializeField] public UnityEvent onLaughedDuringPunchline;
    [SerializeField] public UnityEvent onLaughedOutsidePunchline;

    //Joke Stuff
    [Header("Joke Stuff")]
    [SerializeField] private List<string> possibleLaughs;
    private JokeSO currentJoke;
    private bool speechRunning;
    private float responseDelayBuffer = 0;
    public bool punchlineDelivered;
    public bool laughedDuringPunchline;

    //Timer
    private float punchlineTimer;

    //Responses
    private CapturedPlayerResponse currentResponse = null;
    private List<CapturedPlayerResponse> capturedResponses = new List<CapturedPlayerResponse>();

    private void Awake()
    {
        // Make sure language models exist.
        if (languageModelProvider.languageModels.Count == 0)
        {
            throw new InvalidOperationException("No language models.");
        }

        // Set default language.
        languageModelProvider.language = SystemLanguage.English;
        InitializeActivityDetector();

        // Bind recognizer to event handlers.
        recognizer.Started.AddListener(() =>
        {
            _recognizedText.Clear();
        });

        recognizer.Finished.AddListener(() => Debug.Log("Finished"));
    }

    private void Update()
    {
        if(punchlineTimer > 0)
        {
            punchlineTimer -= Time.deltaTime;
        }
    }
    
    public void RunPunchline(JokeSO joke)
    {
        currentJoke = joke;
        punchlineTimer = currentJoke.punchlineBufferTime;
        capturedResponses = new List<CapturedPlayerResponse>();
    }

    private void InitializeActivityDetector()
    {
        activityDetector.TimeoutMs = 0;
        activityDetector.Spoke.AddListener(() => {
            if(!speechRunning)
            {
                speechRunning = true;
                currentResponse = new CapturedPlayerResponse(DateTime.Now);
            }
        });
        activityDetector.Silenced.AddListener(() => {
            if (speechRunning)
            {
                if (currentResponse != null)
                {
                    currentResponse.endTime = DateTime.Now;
                    currentResponse.responseText = _recognizedText.CurrentText;
                    capturedResponses.Add(currentResponse);
                    CheckIfLaugh(currentResponse);
                }
                _recognizedText.Clear();
                speechRunning = false;
            }
        });
        activityDetector.StartProcessing();
    }
    


    private void CheckIfLaugh(CapturedPlayerResponse response)
    {
        foreach(string laugh in possibleLaughs)
        {
            if (response.responseText.Contains(laugh))
            {
                if (punchlineDelivered)
                {
                    punchlineDelivered = false;
                    laughedDuringPunchline = true;
                    onLaughedDuringPunchline?.Invoke();
                }else 
                {
                    print("Not during punchline");
                    laughedDuringPunchline = false;
                    onLaughedOutsidePunchline?.Invoke();
                }
                OnHasLaughed(response);
                return;
            }
        }
        onLaughedOutsidePunchline?.Invoke();
    }

    /// <summary>
    /// Call this with a player response to determine if we hit the punchline
    /// </summary>
    /// <param name="response"></param>
    public void OnHasLaughed(CapturedPlayerResponse response)
    {
        if (currentJoke == null) return;

        if (punchlineTimer > 0)
        {
            onLaughedDuringPunchline?.Invoke();
        }
        else
        {
            onLaughedOutsidePunchline?.Invoke();
        }
    }
    

    /// <summary>
    ///     Helper class that accumulates recognition results.
    /// </summary>
    private class RecognizedText
    {
        private string _changingText;
        private string _stableText;

        public string CurrentText => $"{_stableText} <color=grey>{_changingText}</color>";

        public void Append(Result result)
        {
            _changingText = "";
            _stableText = $"{_stableText} {result.text}";
        }

        public void Append(PartialResult partialResult)
        {
            _changingText = partialResult.partial;
        }

        public void Clear()
        {
            _changingText = "";
            _stableText = "";
        }
    }
}
