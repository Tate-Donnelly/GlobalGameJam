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

    [Header("Callbacks")]
    [SerializeField] public UnityEvent onLaughedDuringPunchline;
    [SerializeField] public UnityEvent onLaughedOutsidePunchline;

    //Joke Stuff
    [Header("Joke Stuff")]
    [SerializeField] private List<string> possibleLaughs;
    private JokeSO currentJoke;
    private bool speechRunning;
    private float responseDelayBuffer = 0;
    public DateTime laughed;
    public bool hasLaughed;

    //Timer
    private float punchlineTimer;
    private DateTime punchlineStartTime;

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
        UpdateStatus("");
        InitializeActivityDetector();

        // Bind recognizer to event handlers.
        recognizer.Started.AddListener(() =>
        {
            _recognizedText.Clear();
            UpdateStatus("");
        });

        recognizer.Finished.AddListener(() => Debug.Log("Finished"));

        recognizer.PartialResultReady.AddListener(OnPartialResult);
        recognizer.ResultReady.AddListener(OnResult);

        recognizer.InitializationFailed.AddListener(OnError);
        recognizer.RuntimeFailed.AddListener(OnError);
    }

    private void Update()
    {
        if(punchlineTimer > 0)
        {
            punchlineTimer -= Time.deltaTime;
        }
    }
    
    public void RunJoke(JokeSO joke)
    {
        hasLaughed = false;
        currentJoke = joke;
        capturedResponses = new List<CapturedPlayerResponse>();
    }
    
    public void RunPunchline()
    {
        punchlineStartTime = DateTime.Now;
        punchlineTimer = currentJoke.punchlineBufferTime;
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
                laughed = response.startTime;
                OnHasLaughed();
                return;
            }
        }
        onLaughedOutsidePunchline?.Invoke();
    }

    public void ManualLaugh()
    {
        laughed=DateTime.Now;
        OnHasLaughed();
    }

    /// <summary>
    /// Call this with a player response to determine if we hit the punchline
    /// </summary>
    public void OnHasLaughed()
    {
        hasLaughed = true;
        if (currentJoke == null) return;
    
        float diff=(float) laughed.Subtract(punchlineStartTime).TotalSeconds-currentJoke.punchlineBufferTime;
        // If punchline started before response
        if (diff>0 )
        {
            onLaughedOutsidePunchline?.Invoke();
        }else{
            onLaughedDuringPunchline?.Invoke();
        }
    }

    private void UpdateStatus(string text)
    {
        //status.text = text;
    }

    private void OnPartialResult(PartialResult partial)
    {
        _recognizedText.Append(partial);
        UpdateStatus(_recognizedText.CurrentText);
    }

    private void OnResult(Result result)
    {
        _recognizedText.Append(result);
        UpdateStatus(_recognizedText.CurrentText);
    }

    private void OnError(SpeechProcessorException exception)
    {
        UpdateStatus($"<color=red>{exception.Message}</color>");
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