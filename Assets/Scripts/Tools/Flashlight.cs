using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Tool
{

    [SerializeField]
    private Transform dickyTransform;

    private bool flashlightOn = false;
    [SerializeField] AudioSource flashlightSFX;
    private int mask;

    private void Start()
    {
        this.type = ToolType.FLASHLIGHT;
        mask = LayerMask.NameToLayer("Barrier") |
                LayerMask.NameToLayer("Player") |
                LayerMask.NameToLayer("Lights");
    }

    private bool dickySeesPoint(Vector3 position) {
        RaycastHit hit;
        if (!Physics.Raycast(position, 
            Vector3.Normalize(dickyTransform.position - position),
            out hit, Mathf.Infinity, mask)) {
            return false;
        }
        Debug.DrawLine(position, hit.point, Color.white, 10.0f, true);
        return hit.transform.gameObject == dickyTransform.gameObject;
    }

    private bool checkDickySeesLight() {
        if (dickySeesPoint(transform.position)) return true;

        // Uncomment this if you want Dicky to be able to see your light on the back wall

        // Check if dicky can see the other side of the flashlight
        // RaycastHit wallPosition;
        // if (!Physics.Raycast(transform.position, transform.forward, out wallPosition, Mathf.Infinity, mask)) { 
        //     return false; 
        // }
        // float diff = (wallPosition.point - transform.position).magnitude;
        // Vector3 checkPos = transform.position + transform.forward * (diff - 1); 
        // Debug.DrawLine(transform.position, wallPosition.point, Color.red, 10.0f, true);
        // Debug.DrawLine(transform.position, checkPos, Color.green, 10.0f, true);

        // return dickySeesPoint(checkPos);
         return false;    
    }

    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.FLASHLIGHT) return;
        flashlightOn = !flashlightOn;
        flashlightSFX.Play();
        this.gameObject.SetActive(flashlightOn);
    }

    void Update() {
        if (!flashlightOn) return;
        if (checkDickySeesLight()) {
            Debug.Log("Killed");
            FlagSystem.KillPlayer(PuzzleFlag.KEY);
        }
    }
}
