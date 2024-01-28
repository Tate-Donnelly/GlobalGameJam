using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Tool
{
    private void Start()
    {
        this.type = ToolType.KEY;
    }
    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.KEY) return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction * 20);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            if (hitData.collider.tag == "Interactable" && hitData.collider.gameObject.name == "Exit")
            {
                hitData.collider.GetComponent<IInteractable>().InteractAction(this.type);
            }
        }
    }
}
