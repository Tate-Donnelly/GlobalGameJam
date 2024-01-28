using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Tool
{
    private void Start()
    {
        this.type = ToolType.HAND;
    }
    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.HAND) return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction * 5);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            if (hitData.collider.tag == "Interactable")
            {
                hitData.collider.GetComponent<IInteractable>().InteractAction(this.type);
            }
        }
    }
}
