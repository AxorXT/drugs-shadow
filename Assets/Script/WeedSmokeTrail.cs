using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WeedSmokeTrail : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform targetPoint;
    public ParticleSystem smokeParticles;

    [Header("Velocidad")]
    [Range(1f, 20f)]
    public float travelSpeed = 5f;

    [Header("Emisión")]
    public float emissionRate = 25f;
    public float smokeHeightOffset = 1.5f;

    [Header("Ondulación")]
    public float waviness = 0.3f;
    public float waveFrequency = 1.5f;

    private Transform smokeHead;
    private bool isPlaying = false;
    private Coroutine trailCoroutine;

    void Start()
    {
        smokeHead = new GameObject("SmokeHead").transform;
        smokeHead.SetParent(null);
        smokeParticles.Stop();
        smokeParticles.Clear();
    }

    public void PlayTrail()
    {
        if (isPlaying) return;
        if (trailCoroutine != null) StopCoroutine(trailCoroutine);
        trailCoroutine = StartCoroutine(TrailSequence());
    }

    public void StopTrail()
    {
        if (trailCoroutine != null) StopCoroutine(trailCoroutine);
        StartCoroutine(FadeOutTrail());
    }

    IEnumerator TrailSequence()
    {
        isPlaying = true;

        // Calcular el path por NavMesh
        List<Vector3> waypoints = GetNavMeshPath(
            player.position,
            targetPoint.position
        );

        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogWarning("WeedSmokeTrail: no se encontró path al objetivo.");
            isPlaying = false;
            yield break;
        }

        // Configurar emisión
        var emission = smokeParticles.emission;
        emission.rateOverTime = emissionRate;
        smokeHead.position = waypoints[0] + Vector3.up * smokeHeightOffset;
        smokeParticles.transform.position = smokeHead.position;
        smokeParticles.Play();

        // Recorrer cada segmento del path
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 segStart = waypoints[i] + Vector3.up * smokeHeightOffset;
            Vector3 segEnd = waypoints[i + 1] + Vector3.up * smokeHeightOffset;

            float segLength = Vector3.Distance(segStart, segEnd);
            float segTime = segLength / travelSpeed;
            float elapsed = 0f;

            Vector3 direction = (segEnd - segStart).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);

            while (elapsed < segTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / segTime);

                Vector3 basePos = Vector3.Lerp(segStart, segEnd, t);

                // Ondulación orgánica
                float wave = Mathf.Sin(Time.time * waveFrequency * Mathf.PI * 2f) * waviness;
                float waveUp = Mathf.Sin(Time.time * waveFrequency * 1.3f * Mathf.PI * 2f) * (waviness * 0.4f);

                smokeHead.position = basePos
                    + perpendicular * wave
                    + Vector3.up * waveUp;

                smokeParticles.transform.position = smokeHead.position;
                smokeParticles.transform.LookAt(segEnd);

                yield return null;
            }
        }

        // Llegó al destino
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeOutTrail());
        isPlaying = false;
    }

    // Calcula el camino usando NavMesh y devuelve los waypoints
    List<Vector3> GetNavMeshPath(Vector3 from, Vector3 to)
    {
        NavMeshPath path = new NavMeshPath();

        // Buscar los puntos más cercanos en el NavMesh
        NavMesh.SamplePosition(from, out NavMeshHit hitFrom, 2f, NavMesh.AllAreas);
        NavMesh.SamplePosition(to, out NavMeshHit hitTo, 2f, NavMesh.AllAreas);

        bool found = NavMesh.CalculatePath(
            hitFrom.position,
            hitTo.position,
            NavMesh.AllAreas,
            path
        );

        if (!found || path.status == NavMeshPathStatus.PathInvalid)
            return null;

        return new List<Vector3>(path.corners);
    }

    IEnumerator FadeOutTrail()
    {
        var emission = smokeParticles.emission;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.5f;
            emission.rateOverTime = Mathf.Lerp(emissionRate, 0f, t);
            yield return null;
        }

        smokeParticles.Stop();
        smokeParticles.Clear();
        isPlaying = false;
    }

    void OnDestroy()
    {
        if (smokeHead != null)
            Destroy(smokeHead.gameObject);
    }
}