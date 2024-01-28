using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour, IPickupable
{
    [SerializeField] PlayerController player;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] AudioSource jingle;

    string IPickupable.GetName()
    {
        return "key";
    }

    void IPickupable.Pickup()
    {
        player.EnableKey();
        jingle.Play();
        StartCoroutine(DisableKey());
    }

    IEnumerator DisableKey()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
