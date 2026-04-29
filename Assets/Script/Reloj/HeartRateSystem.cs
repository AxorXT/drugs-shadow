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

    void Update()
    {
        Recover();
        CheckDeath();

        if (debug)
            Debug.Log("HeartRate: " + heartRate);
    }

    void Recover()
    {
        heartRate = Mathf.Lerp(heartRate, restingRate, Time.deltaTime * recoverySpeed);
        onHeartRateChanged?.Invoke(heartRate);
    }

    public void AddStress(float amount)
    {
        heartRate += amount;
        heartRate = Mathf.Clamp(heartRate, restingRate, maxHeartRate);
        onHeartRateChanged?.Invoke(heartRate);
    }

    void CheckDeath()
    {
        if (heartRate >= maxHeartRate)
        {
            onDeath?.Invoke();
        }
    }

    public float GetNormalized()
    {
        return heartRate / maxHeartRate;
    }
}
