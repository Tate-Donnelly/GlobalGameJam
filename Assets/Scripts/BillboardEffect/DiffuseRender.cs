using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffuseRender : MonoBehaviour
{
    [SerializeField] SpriteRenderer render;

    // Start is called before the first frame update
    void Start()
    {
        render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        render.receiveShadows = true;
    }
}
