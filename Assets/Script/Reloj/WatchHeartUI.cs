using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchHeartUI : MonoBehaviour
{
    [Header("Referencia")]
    public HeartRateSystem heartSystem;

    [Header("UI")]
    public TextMeshProUGUI bpmText;        // número (85)
    public RectTransform heartIcon;        // corazón
    public RectTransform ecgLine;          // línea ECG

    [Header("Animación")]
    public float ecgBaseSpeed = 50f;
    public float pulseIntensity = 0.1f;

    void Start()
    {
        heartSystem.onHeartRateChanged += UpdateUI;
    }

    void Update()
    {
        AnimateHeart();
        AnimateECG();
    }

    void UpdateUI(float currentRate)
    {
        // Texto BPM
        bpmText.text = Mathf.RoundToInt(currentRate).ToString();
    }

    void AnimateHeart()
    {
        float pulseSpeed = heartSystem.heartRate * 0.1f;
        float scale = 0.3f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;

        heartIcon.localScale = Vector3.one * scale;
    }

    void AnimateECG()
    {
        float speed = ecgBaseSpeed + heartSystem.heartRate;

        ecgLine.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        if (ecgLine.anchoredPosition.x < -200f)
        {
            ecgLine.anchoredPosition = new Vector2(200f, ecgLine.anchoredPosition.y);
        }
    }
}
