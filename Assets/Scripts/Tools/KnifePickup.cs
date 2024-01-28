using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePickup : MonoBehaviour, IPickupable
{
    [SerializeField] PlayerController player;

    string IPickupable.GetName()
    {
        return "knife";
    }

    void IPickupable.Pickup()
    {
        player.EnableKnife();
        this.gameObject.SetActive(false);
    }
}
