using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour, IInteractable
{
    [Header("Objects Connected")]
    [SerializeField] List<GameObject> spotlights;
    [SerializeField] ParticleSystem particles;

    public void InteractAction()
    {
        foreach(GameObject light in spotlights)
        {
            light.SetActive(true);
            particles.Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
