using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WatchHeartUI : MonoBehaviour
{
    [Header("Referencia")]
    public HeartRateSystem heartSystem;

    [Header("UI")]
    public TextMeshProUGUI bpmText;
    public RectTransform heartIcon;

    [Header("ECG")]
    public RectTransform ecg1;
    public RectTransform ecg2;
    public float width = 400f;
    public float duration = 2f;

    [Header("Animación")]
    public float pulseIntensity = 0.1f;

    private Tween tween1;
    private Tween tween2;

    void Start()
    {
        heartSystem.onHeartRateChanged += UpdateUI;
        StartECGLoop();
    }

    void Update()
    {
        AnimateHeart();
        UpdateSpeed();
    }

    void UpdateUI(float currentRate)
    {
        bpmText.text = Mathf.RoundToInt(currentRate).ToString();
    }

    void AnimateHeart()
    {
        float pulseSpeed = heartSystem.heartRate * 0.1f;
        float scale = 0.3f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        heartIcon.localScale = Vector3.one * scale;
    }

    void StartECGLoop()
    {
        ecg1.anchoredPosition = new Vector2(0, ecg1.anchoredPosition.y);
        ecg2.anchoredPosition = new Vector2(width, ecg2.anchoredPosition.y);

        tween1 = ecg1.DOAnchorPosX(-width, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        tween2 = ecg2.DOAnchorPosX(0, duration)
            .From(new Vector2(width, ecg2.anchoredPosition.y))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    void UpdateSpeed()
    {
        float speedMultiplier = Mathf.Lerp(0.5f, 2f, heartSystem.GetNormalized());

        if (tween1 != null) tween1.timeScale = speedMultiplier;
        if (tween2 != null) tween2.timeScale = speedMultiplier;
    }
}