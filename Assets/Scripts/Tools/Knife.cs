using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Tool
{
    private void Start()
    {
        this.type = ToolType.KNIFE;
    }

    public override void InteractAction(ToolType type)
    {
        if (type != ToolType.KNIFE) return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction * 5);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 2f))
        {
            if (hitData.collider.tag == "Interactable" && hitData.collider.gameObject.name == "Correct Rope")
            {
                hitData.collider.GetComponent<IInteractable>().InteractAction(this.type);
            }
        }
    }
}
