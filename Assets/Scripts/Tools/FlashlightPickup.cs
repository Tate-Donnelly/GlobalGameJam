using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightPickup : MonoBehaviour, IPickupable
{
    [SerializeField]
    PlayerController player;

    string IPickupable.GetName()
    {
        return "flashlight";
    }

    void IPickupable.Pickup()
    {
        player.EnableFlashlight();
        this.gameObject.SetActive(false);
    }
}
