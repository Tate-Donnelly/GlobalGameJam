using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlickerLights : MonoBehaviour
{
    private const string EMISSIVE_COLOR_NAME = "_EmissionColor";
    private const string EMISSIVE_KEYWORD = "_EMISSION";

    [SerializeField] Renderer rend;
    private List<Material> materials = new();
    private List<Color> initialColors = new();

    [Header("Flicker Params")]
    [SerializeField] private bool flicker;
    [SerializeField] [Min(0)] private float flickerSpeed = 1f;
    [SerializeField] private AnimationCurve brightnessCurve;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        brightnessCurve.postWrapMode = WrapMode.Loop;

        foreach(Material material in rend.materials)
        {
            if (rend.material.enabledKeywords.Any(item => item.name == EMISSIVE_KEYWORD)
                && rend.material.HasColor(EMISSIVE_COLOR_NAME))
            {
                materials.Add(material);
                initialColors.Add(material.GetColor(EMISSIVE_COLOR_NAME));
            }
            else
            {
                Debug.LogWarning($"{material.name} is not configured to be emissive. " +
                    $"so FlickeringEmissive on {name} cannot animate this material!");
            }
        }

        if (materials.Count == 0)
        {
            enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //DynamicGI.SetEmissive(rend, emColor);

        if (flicker && rend.isVisible)
        {
            float scaledTime = Time.time * flickerSpeed;

            for(int i = 0; i < materials.Count; i++)
            {
                Color color = initialColors[i];

                float brightness = brightnessCurve.Evaluate(scaledTime);
                color = new Color(
                    color.r * Mathf.Pow(2, brightness),
                    color.g * Mathf.Pow(2, brightness),
                    color.b * Mathf.Pow(2, brightness),
                    color.a
                );

                materials[i].SetColor(EMISSIVE_COLOR_NAME, color);
                DynamicGI.SetEmissive(rend, color);
            }
        }
    }
}
