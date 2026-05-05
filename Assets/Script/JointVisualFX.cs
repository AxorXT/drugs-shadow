using DG.Tweening;
using UnityEngine;

public class JointVisualFX : MonoBehaviour
{
    public Renderer jointRenderer;

    private Material mat;

    public Color offColor = Color.white;
    public Color onColor = new Color(1f, 0.3f, 0.1f);

    public Color idleColor = new Color(0.4f, 0.1f, 0.05f);
    public float idleIntensity = 0.5f;

    public float intensity = 3f;
    public float glowDuration = 0.3f;
    public ParticleSystem smokePuff;

    void Start()
    {
        mat = jointRenderer.material;
        SetIdleGlow();
    }

    public void SetOn()
    {
        mat.DOColor(onColor * intensity, "_EmissionColor", glowDuration);
    }

    public void SetOff()
    {
        mat.DOColor(offColor, "_EmissionColor", glowDuration);
    }

    public void PlaySmoke()
    {
        if (smokePuff != null)
            smokePuff.Play();
    }

    public void SetIdleGlow()
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", idleColor * idleIntensity);
    }

    public void SetOffline()
    {
        if (mat != null)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }

        if (smokePuff != null)
            smokePuff.Stop();
    }
}