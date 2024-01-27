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
        currentJoke = joke;
        UpdateStatus("");
        punchlineStartTime = DateTime.Now;
        punchlineTimer = currentJoke.punchlineBufferTime;
        capturedResponses = new List<CapturedPlayerResponse>();
    }

    private void InitializeActivityDetector()
    {
        activityDetector.TimeoutMs = 0;
        activityDetector.Spoke.AddListener(() => {
            //hearStatus.text = "<color=green>Speech</color>";
            if(!speechRunning)
            {
                UpdateStatus("Laugh!");
                speechRunning = true;
                currentResponse = new CapturedPlayerResponse(DateTime.Now);
            }
        });
        activityDetector.Silenced.AddListener(() => {
            //hearStatus.text = "<color=red>Silence</color>";
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
                UpdateStatus("");
                speechRunning = false;
            }
        });
        //activityDetector.InitializationFailed.AddListener(e => hearStatus.text = e.Message);
        activityDetector.StartProcessing();
    }
    
    public void DeliveredPunchline()
    {
        print("Delivered punchline");
        //jokeStartTime = currentJoke.dialogueDuration;
    }


    private void CheckIfLaugh(CapturedPlayerResponse response)
    {
        foreach(string laugh in possibleLaughs)
        {
            if (response.responseText.Contains(laugh))
            {
                print("HeardLaughter");
                if (punchlineDelivered)
                {
                    print("Punchline");
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
