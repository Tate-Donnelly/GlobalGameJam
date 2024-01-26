using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventTrigger {
    ON_ENTER,
    ON_EXIT,
    ON_STAY
};

public class FlagTrigger : MonoBehaviour
{
    [SerializeField]
    private PuzzleFlag flag;
    
    [SerializeField]
    private EventTrigger trigger;

    void OnTriggerEnter(Collider other) {
        if (trigger != EventTrigger.ON_ENTER) return;
        FlagSystem.NotifyFlag(flag);
    }

    void OnTriggerExit(Collider other) {
        if (trigger != EventTrigger.ON_EXIT) return;
        FlagSystem.NotifyFlag(flag);
    }

    void OnTriggerStay(Collider other) {
        if (trigger != EventTrigger.ON_STAY) return;
        FlagSystem.NotifyFlag(flag);
    }
}
