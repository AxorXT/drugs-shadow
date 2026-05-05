using UnityEngine;

public class HeartRateSystem : MonoBehaviour
{
    [Header("Valores")]
    public float heartRate = 70f;
    public float restingRate = 65f;
    public float maxHeartRate = 180f;

    [Header("Velocidades")]
    public float recoverySpeed = 5f;

    [Header("Debug")]
    public bool debug;

    public System.Action<float> onHeartRateChanged;
    public System.Action onDeath;

    bool isDead = false;
    float currentBaseRate;

    void Start()
    {
        currentBaseRate = restingRate;
    }

    void Update()
    {
        Recover();
        CheckDeath();

        if (debug)
            Debug.Log("HeartRate: " + heartRate);
    }

    void Recover()
    {
        heartRate = Mathf.MoveTowards(
            heartRate,
            currentBaseRate,
            Time.deltaTime * recoverySpeed
        );

        onHeartRateChanged?.Invoke(heartRate);
    }

    public void AddStress(float amount)
    {
        heartRate += amount;
        heartRate = Mathf.Clamp(heartRate, restingRate, maxHeartRate);

        //subir la base si el pulso sube más
        if (heartRate > currentBaseRate)
        {
            currentBaseRate = heartRate;
        }

        onHeartRateChanged?.Invoke(heartRate);
    }

    void CheckDeath()
    {
        if (heartRate >= maxHeartRate && !isDead)
        {
            isDead = true;
            onDeath?.Invoke();
        }
    }

    public float GetNormalized()
    {
        return heartRate / maxHeartRate;
    }

    public void ResetState()
    {
        heartRate = restingRate;
        currentBaseRate = restingRate;
        isDead = false;

        onHeartRateChanged?.Invoke(heartRate);
    }
}
