using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Range(0f, 0.5f)]
    public float flickerAmount = 0.1f;   // 0.1 = ±10%

    [Tooltip("How fast it flickers")]
    public float flickerSpeed = 12f;

    private Light lightSource;
    private float originalIntensity;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        originalIntensity = lightSource.intensity;
    }

    void Update()
    {
        // Smooth Perlin noise flicker
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        float offset = (noise - 0.5f) * 2f * flickerAmount;

        lightSource.intensity = originalIntensity * (1f + offset);
    }
}