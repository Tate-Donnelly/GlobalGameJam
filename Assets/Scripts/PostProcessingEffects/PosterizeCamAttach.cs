using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class PosterizeCamAttach : MonoBehaviour
{
    [SerializeField]
    private Material material;
    
    void Start() {
        if (material == null || material.shader == null || !material.shader.isSupported) {
            Debug.LogWarning("Posterize material improperly set");
            enabled = false;
            return;
        }

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, material);
    }
}
