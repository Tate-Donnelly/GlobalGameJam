using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour, IPickupable
{
    [SerializeField] PlayerController player;

    string IPickupable.GetName()
    {
        return "key";
    }

    void IPickupable.Pickup()
    {
        player.EnableKey();
        this.gameObject.SetActive(false);
    }
}
