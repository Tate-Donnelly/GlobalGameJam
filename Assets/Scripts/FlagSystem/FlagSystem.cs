using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PuzzleFlag {
   SWITCH,
   UNTIE,
   SANDBAG,
   KEY,
   DEATH,
   NONE // Used for cause of death
};


// Flag args, IF NOT DEATH THEN DEATH CAUSE WILL BE NONE
public class FlagArgs : EventArgs {
    public PuzzleFlag flag {get; private set;}
    public PuzzleFlag deathCause {get; private set;}

    public FlagArgs(PuzzleFlag flag, PuzzleFlag deathCause) {
       this.flag = flag;
       this.deathCause = deathCause;
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
    public static HashSet<PuzzleFlag> FlagsNotified
    {
        get
        {
            return instance.flagsNotified;
        }
    }

    public static void NotifyFlag(PuzzleFlag flag) {
        instance.notifyFlag(flag, PuzzleFlag.NONE);
    }

    public static void KillPlayer(PuzzleFlag deathCause) {
        instance.notifyFlag(PuzzleFlag.DEATH, deathCause);
    }

    private void notifyFlag(PuzzleFlag flag, PuzzleFlag deathCause) {
        if(flagsNotified.Contains(flag)) return;
        flagsNotified.Add(flag);
        var flagArgs = new FlagArgs(flag, deathCause);
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
