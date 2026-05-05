using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using DG.Tweening;


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
    Vector3 originalCamPos;

    public InputManager input;
    public WeedSmokeTrail smokeTrail;
    public HeartRateSystem heartSystem;

    public SmokingHandAnimator smokingHand;
    public JointVisualFX jointFX;
    public ShadowSystem shadowSystem;

    [Header("Weed Progression")]
    public float weedIntensity = 0f;
    public float maxWeed = 1f;
    public float increaseAmount = 0.25f;
    public float decaySpeed = 0.05f;

    void Start()
    {
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out colorAdjust);
        volume.profile.TryGet(out vignette);
        ResetValues();
        originalCamPos = Camera.main.transform.localPosition;
    }

    void Update()
    {
        bool isPressed = input.weedPressed;
        if (!isActive)
        {
            weedIntensity = Mathf.MoveTowards(weedIntensity, 0f, Time.deltaTime * decaySpeed);
        }

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
        heartSystem.recoverySpeed = 1.5f;
        weedIntensity += increaseAmount;
        weedIntensity = Mathf.Clamp01(weedIntensity);

        yield return smokingHand.ToSmoke().WaitForCompletion();
        yield return new WaitForSeconds(0.3f);

        jointFX.SetOn();
        jointFX.PlaySmoke();

        shadowSystem.ClearShadows();
        heartSystem.AddStress(10f);
        smokeTrail.PlayTrail();

        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
            shadowSystem.SpawnShadow();

        yield return StartCoroutine(ActivateEffect());

        StartCoroutine(ReturnHandToIdleAfterDelay(2.5f));

        if (loopEffect == null)
            loopEffect = StartCoroutine(WeedLoop());

        yield return new WaitForSeconds(effectDuration);
        smokeTrail.StopTrail();

        //DETENER LOOP
        if (loopEffect != null)
        {
            StopCoroutine(loopEffect);
            loopEffect = null;
        }

        //APAGAR EFECTO VISUAL
        yield return StartCoroutine(DeactivateEffect());

        //limpiar estado
        isActive = false;
        heartSystem.recoverySpeed = 5f;
    }

    IEnumerator ActivateEffect()
    {
        volume.weight = 1f;

        lens.active = true;
        chroma.active = true;
        colorAdjust.active = true;
        vignette.active = true;

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
            float intensity = Mathf.Clamp01(weedIntensity);
            float time = Time.time;

            lens.intensity.value =
                (-0.6f + Mathf.Sin(time * 6f) * 0.15f);

            chroma.intensity.value =
                Mathf.Lerp(0f, 1f, intensity) +
                Mathf.Sin(time * 2f) * 0.15f * intensity;

            colorAdjust.postExposure.value =
                Mathf.Lerp(0f, 0.4f, intensity);

            colorAdjust.hueShift.value =
                Mathf.Sin(time * 5f) * 60f * intensity;

            vignette.intensity.value = (0.4f + Mathf.Sin(time * 6f) * 0.1f);

            Camera.main.fieldOfView =
                originalFOV +
                Mathf.Lerp(0f, 18f, intensity);

            Camera.main.transform.localPosition =
                originalCamPos +
                new Vector3(
                    Mathf.Sin(time * 40f) * 0.02f * intensity,
                    Mathf.Sin(time * 35f) * 0.02f * intensity,
                    0
                );

            float baseStress = 1.5f;          // siempre sube aunque estés leve
            float intensityStress = 4f;    // escala con droga

            heartSystem.AddStress(
                Time.deltaTime * (baseStress + intensity * intensityStress)
            );

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

        if (lens != null)
        {
            lens.intensity.value = 0f;
            lens.active = false;
        }

        if (chroma != null)
        {
            chroma.intensity.value = 0f;
            chroma.active = false;
        }

        if (colorAdjust != null)
        {
            colorAdjust.saturation.value = 0f;
            colorAdjust.postExposure.value = 0f;
            colorAdjust.hueShift.value = 0f;
            colorAdjust.active = false;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0f;
            vignette.active = false;
        }

        if (Camera.main != null)
        {
            Camera.main.fieldOfView = originalFOV;
            Camera.main.transform.localPosition = originalCamPos;
        }
    }

    public void ForceStopEffect()
    {
        if (currentEffect != null)
            StopCoroutine(currentEffect);

        if (loopEffect != null)
            StopCoroutine(loopEffect);

        currentEffect = null;
        loopEffect = null;

        smokeTrail.StopTrail();

        //SOLO visuales
        ResetValues();

        isActive = false;
    }

    IEnumerator ReturnHandToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        smokingHand.ToIdle();
    }

    public void FullReset()
    {
        // detener coroutines
        if (currentEffect != null)
            StopCoroutine(currentEffect);

        if (loopEffect != null)
            StopCoroutine(loopEffect);

        currentEffect = null;
        loopEffect = null;

        // estado
        isActive = false;
        weedIntensity = 0f;

        // parar efectos visuales
        ResetValues();

        // detener humo
        if (smokeTrail != null)
            smokeTrail.StopTrail();

        // apagar joint visual
        if (jointFX != null)
            jointFX.SetOffline();

        // regresar mano INSTANTÁNEAMENTE
        if (smokingHand != null)
            smokingHand.SetInstantIdle();

        // limpiar sombras
        if (shadowSystem != null)
            shadowSystem.ClearShadows();
    }
}
