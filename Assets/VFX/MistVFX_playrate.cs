using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways] // <-- Add this line so it works outside of Play Mode
public class MistVFX_playrate : MonoBehaviour
{
    private VisualEffect vfx;

    [Range(0f, 2f)] public float playRate = 0.5f;

    // Use OnValidate to ensure the component is grabbed when values change in inspector
    private void OnValidate()
    {
        if (vfx == null) vfx = GetComponent<VisualEffect>();
    }

    void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (vfx != null)
        {
            vfx.playRate = playRate;
        }
    }
}