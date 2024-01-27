using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PuzzleFlag {
   SWITCH,
   UNTIE,
   SANDBAG,
   KEY
};

public class FlagArgs : EventArgs {
    public PuzzleFlag flag {get; private set;}
    
    public FlagArgs(PuzzleFlag flag) {
       this.flag = flag;
    }
}

[Serializable]
public class FlagUnityEvent : UnityEvent<FlagArgs> {}

public class FlagSystem : MonoBehaviour
{
    public static FlagSystem instance {get; private set;}
    public static event EventHandler<FlagArgs> OnFlagNotified; 

    public FlagUnityEvent OnFlagNotifiedUnityEvent;

    private HashSet<PuzzleFlag> flagsNotified;

    public static void NotifyFlag(PuzzleFlag flag) {
        instance.notifyFlag(flag);
    }

    private void notifyFlag(PuzzleFlag flag) {
        if(flagsNotified.Contains(flag)) return;
        flagsNotified.Add(flag);
        var flagArgs = new FlagArgs(flag);
        OnFlagNotified?.Invoke(this, flagArgs);
        OnFlagNotifiedUnityEvent?.Invoke(flagArgs);
    }

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
        flagsNotified = new HashSet<PuzzleFlag>();
        DontDestroyOnLoad(gameObject);
    }
}
