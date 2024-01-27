using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleFlag {
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

public class FlagSystem : MonoBehaviour
{
    public static FlagSystem instance {get; private set;}
    public static event EventHandler<FlagArgs> OnFlagNotified; 

    private HashSet<PuzzleFlag> flagsNotified;

    public static void NotifyFlag(PuzzleFlag flag) {
        instance.notifyFlag(flag);
    }

    private void notifyFlag(PuzzleFlag flag) {
        if(flagsNotified.Contains(flag)) return;
        flagsNotified.Add(flag);
        OnFlagNotified?.Invoke(this, new FlagArgs(flag));
    }

    void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
        flagsNotified = new HashSet<PuzzleFlag>();
        DontDestroyOnLoad(gameObject);
    }
}
