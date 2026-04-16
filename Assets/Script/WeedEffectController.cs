using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;


public class WeedEffectController : MonoBehaviour
{
    public Volume volume;

    private LensDistortion lens;
    private ChromaticAberration chroma;
    private ColorAdjustments colorAdjust;

    public float transitionSpeed = 2f;

    private Coroutine currentEffect;
    public InputManager input;

    void Start()
    {
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out colorAdjust);

        ResetValues();
    }

    void Update()
    {
        if (input.weedPressed)
        {
            ToggleEffect();
        }
    }

    void ToggleEffect()
    {
        if (currentEffect != null)
            StopCoroutine(currentEffect);

        if (lens.intensity.value == 0)
            currentEffect = StartCoroutine(ActivateEffect());
        else
            currentEffect = StartCoroutine(DeactivateEffect());
    }

    IEnumerator ActivateEffect()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            lens.intensity.value = Mathf.Lerp(0, -0.5f, t);
            chroma.intensity.value = Mathf.Lerp(0, 1f, t);
            colorAdjust.saturation.value = Mathf.Lerp(0, -40f, t);
            colorAdjust.hueShift.value = Mathf.Sin(Time.time * 2f) * 20f;
            Camera.main.fieldOfView = Mathf.Lerp(60, 75, t);

            yield return null;
        }
    }

    IEnumerator DeactivateEffect()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            lens.intensity.value = Mathf.Lerp(-0.5f, 0, t);
            chroma.intensity.value = Mathf.Lerp(1f, 0, t);
            colorAdjust.saturation.value = Mathf.Lerp(-40f, 0, t);

            yield return null;
        }
    }

    void ResetValues()
    {
        lens.intensity.value = 0;
        chroma.intensity.value = 0;
        colorAdjust.saturation.value = 0;
    }
}
