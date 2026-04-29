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
    private Vignette vignette;

    public float transitionSpeed = 1.5f;
    public float originalFOV = 60f;
    public float effectDuration = 15f;

    private Coroutine currentEffect;
    private Coroutine loopEffect;
    private bool isActive = false;
    private bool wasPressed = false;

    public InputManager input;
    public WeedSmokeTrail smokeTrail;
    public HeartRateSystem heartSystem;

    void Start()
    {
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out colorAdjust);
        volume.profile.TryGet(out vignette);
        ResetValues();
    }

    void Update()
    {
        bool isPressed = input.weedPressed;
        if (isPressed && !wasPressed && !isActive)
        {
            StartWeedEffect();
        }
        wasPressed = isPressed;
    }

    void StartWeedEffect()
    {
        if (currentEffect != null) StopCoroutine(currentEffect);
        isActive = true;
        currentEffect = StartCoroutine(WeedEffectSequence());
    }

    IEnumerator WeedEffectSequence()
    {
        yield return StartCoroutine(ActivateEffect());
        heartSystem.AddStress(40f);
        smokeTrail.PlayTrail();

        loopEffect = StartCoroutine(WeedLoop());
        yield return new WaitForSeconds(effectDuration);

        if (loopEffect != null) StopCoroutine(loopEffect);
        smokeTrail.StopTrail();
        yield return StartCoroutine(DeactivateEffect());

        isActive = false;
    }

    IEnumerator ActivateEffect()
    {
        float t = 0;
        float startFOV = Camera.main.fieldOfView;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            lens.intensity.value = Mathf.Lerp(0f, -0.6f, ease);
            chroma.intensity.value = Mathf.Lerp(0f, 1f, ease);
            colorAdjust.saturation.value = Mathf.Lerp(0f, -30f, ease);
            colorAdjust.postExposure.value = Mathf.Lerp(0f, 0.4f, ease);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(0f, 0.45f, ease);
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, originalFOV + 18f, ease);

            yield return null;
        }
    }

    IEnumerator WeedLoop()
    {
        while (true)
        {
            float time = Time.time;
            heartSystem.AddStress(Time.deltaTime * 3f);
            colorAdjust.hueShift.value = Mathf.Sin(time * 0.8f) * 25f;
            Camera.main.fieldOfView = (originalFOV + 18f) + Mathf.Sin(time * 1.2f) * 4f;
            lens.intensity.value = -0.6f + Mathf.Sin(time * 1.5f) * 0.1f;
            chroma.intensity.value = 1f + Mathf.Sin(time * 2f) * 0.15f;

            yield return null;
        }
    }

    IEnumerator DeactivateEffect()
    {
        float startLens = lens.intensity.value;
        float startChroma = chroma.intensity.value;
        float startSat = colorAdjust.saturation.value;
        float startExp = colorAdjust.postExposure.value;
        float startHue = colorAdjust.hueShift.value;
        float startFOV = Camera.main.fieldOfView;
        float startVig = vignette != null ? vignette.intensity.value : 0f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            lens.intensity.value = Mathf.Lerp(startLens, 0f, ease);
            chroma.intensity.value = Mathf.Lerp(startChroma, 0f, ease);
            colorAdjust.saturation.value = Mathf.Lerp(startSat, 0f, ease);
            colorAdjust.postExposure.value = Mathf.Lerp(startExp, 0f, ease);
            colorAdjust.hueShift.value = Mathf.Lerp(startHue, 0f, ease);
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, originalFOV, ease);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(startVig, 0f, ease);

            yield return null;
        }

        ResetValues();
    }

    void ResetValues()
    {
        lens.intensity.value = 0f;
        chroma.intensity.value = 0f;
        colorAdjust.saturation.value = 0f;
        colorAdjust.postExposure.value = 0f;
        colorAdjust.hueShift.value = 0f;
        if (vignette != null)
            vignette.intensity.value = 0f;
        if (Camera.main != null)
            Camera.main.fieldOfView = originalFOV;
    }
}
